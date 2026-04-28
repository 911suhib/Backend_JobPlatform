using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Domain.src.Abstractions;
using JobPlatformBackend.Domain.src.Entity;
using JobPlatformBackend.Domain.src.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace JobPlatformBackend.Business.src.Services.Implementations
{
	public class ResumeService : IResumeService
	{
		private readonly IPdfService _pdfService;
		private readonly IDashboardRepository _repo;
		private readonly IGeminiService _aiService; // سنحتاج لبرمجتها
		private readonly IUserRepository _userRepository;
		private readonly ISkillRepository _skillRepository;
		public ResumeService(IPdfService pdfService, IDashboardRepository repo, IGeminiService aiService, IUserRepository userRepository , ISkillRepository skill)
		{
			_pdfService = pdfService;
			_repo = repo;
			_aiService = aiService;
			_userRepository = userRepository;
			_skillRepository = skill;
		}

		public async Task<bool> ProcessResumeAsync(int userId, IFormFile file)
		{
			var isExist = await _userRepository.GetByIdAsync(userId);
			if (isExist is null) throw new BadRequestException("User does not exist");

			var text = _pdfService.ExtractTextFromPdf(file);
			if (string.IsNullOrEmpty(text)) return false;

			// 1. تحليل النص بالذكاء الاصطناعي (اللي عدلناه في الخطوة الأولى)
			var aiResult = await _aiService.AnalyzeResumeAsync(text);

			if (aiResult is null) return false;
			if (!aiResult.isResume) throw new BadRequestException("This is not a valid Resume");

			// 2. تجهيز الـ Roadmap بتنسيق CamelCase للفرونت إند 🛡️
			var roadmapObject = new
			{
				MissingSkills = aiResult.missingSkillsWithImpact,
				Courses = aiResult.roadmapData
			};

			// التعديل المهم: استخدام Newtonsoft لضمان الـ CamelCase في الـ JSON المخزن
			string roadmapJsonString = JsonConvert.SerializeObject(roadmapObject, new JsonSerializerSettings
			{
				ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
			});

			// 3. تحديث بيانات الداشبورد والـ CareerPath
			await _repo.UpdateUserDashboardDataAsync(
				userId,
				aiResult.headline,
				aiResult.experienceYears,
				aiResult.marketValue,
				aiResult.recommendation,
				aiResult.progress,
				aiResult.targetTitle,
				roadmapJsonString
			);

			// 4. تحديث المهارات بدون تكرار (Safe Skill Sync) 🛡️
			var user = await _repo.GetFullDashboardDataAsync(userId);
			if (user != null && aiResult.skills != null && aiResult.skills.Any())
			{
				user.UserSkills.Clear(); // نمسح القديم وننزل الجديد بناءً على تحليل الـ AI

				foreach (var skillName in aiResult.skills)
				{
					// شيك إذا المهارة موجودة أصلاً بالسيستم عشان ما نكررها
					var existingSkill = await _skillRepository.GetByNameAsync(skillName);

					user.UserSkills.Add(new UserSkill
					{
						UserID = user.Id,
						Skill = existingSkill ?? new Skill { Name = skillName }
					});
				}
				await _repo.SaveChangesAsync();
			}

			return true;
		}
	}
}
