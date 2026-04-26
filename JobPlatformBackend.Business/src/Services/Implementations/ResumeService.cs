using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Domain.src.Abstractions;
using JobPlatformBackend.Domain.src.Entity;
using Microsoft.AspNetCore.Http;

using JobPlatformBackend.Domain.src.Exceptions;

namespace JobPlatformBackend.Business.src.Services.Implementations
{
	public class ResumeService : IResumeService
	{
		private readonly IPdfService _pdfService;
		private readonly IDashboardRepository _repo;
		private readonly IGeminiService _aiService; // سنحتاج لبرمجتها
		private readonly IUserRepository _userRepository;
		public ResumeService(IPdfService pdfService, IDashboardRepository repo, IGeminiService aiService, IUserRepository userRepository )
		{
			_pdfService = pdfService;
			_repo = repo;
			_aiService = aiService;
			_userRepository = userRepository;
		}

		public async Task<bool> ProcessResumeAsync(int userId, IFormFile file)
		{
			var isExist = await _userRepository.GetByIdAsync(userId);
			if(isExist is null)
				throw new BadRequestException("This user is not exist");

			var text = _pdfService.ExtractTextFromPdf(file);
			if (string.IsNullOrEmpty(text)) return false;

			// 1. تحليل النص بالذكاء الاصطناعي
			var aiResult = await _aiService.AnalyzeResumeAsync(text);

			if (aiResult is null) return false;
			if (!aiResult.isResume) throw new BadRequestException("This is not Resume");

			// 2. تحديث البيانات باستخدام الميثود المتخصصة في الـ Repo
			// هذه الميثود تضمن الـ Include والـ SaveChanges بشكل صحيح
			await _repo.UpdateUserDashboardDataAsync(
				userId,
				aiResult.headline,
				aiResult.experienceYears,
				aiResult.marketValue,
				aiResult.recommendation,
				aiResult.progress,
				aiResult.targetTitle,
				aiResult.missingSkillsWithImpact
				
			);

			// 3. تحديث المهارات (بما أنها ليست ضمن الميثود أعلاه)
			var user = await _repo.GetFullDashboardDataAsync(userId);
			if (user != null && aiResult.skills != null && aiResult.skills.Any())
			{
				user.UserSkills.Clear();
				foreach (var skillName in aiResult.skills)
				{
					user.UserSkills.Add(new UserSkill
					{
						UserID = user.Id,
						Skill = new Skill { Name = skillName }
					});
				}
				await _repo.SaveChangesAsync();
			}

			return true;
		}
	}
}
