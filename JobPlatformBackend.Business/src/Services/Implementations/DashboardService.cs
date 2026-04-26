using JobPlatformBackend.API.Contracts.User.GetAll;
using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Domain.src.Abstractions;
using JobPlatformBackend.Domain.src.Entity;
using Microsoft.EntityFrameworkCore;

namespace JobPlatformBackend.Business.src.Services.Implementations
{
	public class DashboardService : IDashboardService
	{
		private readonly IDashboardRepository _dashboardRepo;
		// سنحتاج لعمل Repo خاص للـ Stats و الـ Career إذا أردت التعامل معهم مباشرة
		// ولكن بما أننا نستخدم الـ DashboardRepository كـ Aggregate Root للـ User:

		public DashboardService(IDashboardRepository dashboardRepo)
		{
			_dashboardRepo = dashboardRepo;
		}

		public async Task<UserDashboardDto> GetUserDashboardAsync(int userId)
		{
			// Eager Loading من الريبوزيتوري اللي عملناه
			var user = await _dashboardRepo.GetFullDashboardDataAsync(userId);

			if (user == null)
				throw new KeyNotFoundException($"User with ID {userId} was not found.");

			// تحويل البيانات لـ DTO
			return new UserDashboardDto
			{
				FullName = $"{user.FName} {user.LName}",
				Headline = user.Headline ?? "New Talent",
				Skills = user.UserSkills?
							 .Select(us => us.Skill?.Name)
							 .Where(name => name != null)
							 .ToList() ?? new List<string>(),

				// سحب البيانات من الجداول المرتبطة مع التأكد من الـ Null
				Commits = user.DashboardStats?.CodeCommits ?? 0,
				Rank = user.DashboardStats?.SkillRank ?? "Beginner",
				MarketValue = user.DashboardStats?.MarketValue ?? "Pending Analysis",
				Views = user.DashboardStats?.ProfileViews ?? 0,

				TargetTitle = user.CareerPath?.TargetTitle ?? "Not Set",
				Progress = user.CareerPath?.ProgressPct ?? 0,
				AIRecommendation = user.CareerPath?.AIRecommendation ?? "Please upload your CV to start the AI analysis.",
				RoadmapData=user.CareerPath?.RoadmapJson
			};
		}

		public async Task<bool> InitializeDashboardAsync(int userId)
		{
			// هذا المنطق يستدعى عند إنشاء مستخدم جديد (Register)
			// الهدف: إنشاء سجلات فارغة في جداول الـ One-to-One لتجنب الـ Null لاحقاً

			var user = await _dashboardRepo.GetByIdAsync(userId);
			if (user == null) return false;

			// 1. تهيئة جدول الإحصائيات
			user.DashboardStats = new UserDashboardStats
			{
				UserId = userId,
				CodeCommits = 0,
				SkillRank = "Newcomer",
				MarketValue = "Calculating...",
				ProfileViews = 0
			};

			// 2. تهيئة جدول المسار المهني
			// داخل ميثود InitializeDashboardAsync
			user.CareerPath = new CareerArchitect
			{
				UserId = userId,
				TargetTitle = "Career Path Pending",
				ProgressPct = 0,
				AIRecommendation = "Your career journey starts here!",
				RoadmapJson = "[]" // مصفوفة فارغة كـ string
			};

			try
			{
				await _dashboardRepo.UpdateAsync(user);
				await _dashboardRepo.SaveChangesAsync();
				return true;
			}
			catch (Exception)
			{
				// سجل الخطأ هنا
				return false;
			}
		}
	}
}