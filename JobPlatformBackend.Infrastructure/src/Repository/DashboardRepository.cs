using JobPlatformBackend.Contracts.Contracts.AI;
using JobPlatformBackend.Domain.src.Abstractions;
using JobPlatformBackend.Domain.src.Entity;
using JobPlatformBackend.Infrastructure.src.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JobPlatformBackend.Infrastructure.src.Repository
{
	public class DashboardRepository : BaseRepository<User>, IDashboardRepository
	{
		private readonly AppDbContext _context;
		private readonly ILogger _logger;

		public DashboardRepository(AppDbContext context, ILogger<DashboardRepository> logger) : base(context, logger)
		{

			_context = context;
			_logger = logger;
 		}

		public async Task<User?> GetFullDashboardDataAsync(int userId, CancellationToken cancellationToken = default)
		{
 			return await _context.Users
				.Include(u => u.DashboardStats)
				.Include(u => u.CareerPath)
				.Include(u => u.UserSkills)
					.ThenInclude(us => us.Skill)
				.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
		}
		public async Task UpdateUserDashboardDataAsync(int userId, string headline, string exp, string marketValue, string recommendation, int progres, string targetTitle, string roadMap)
		{
			// 1. جلب اليوزر الأساسي مع العلاقات
			var user = await _context.Users
				.Include(u => u.DashboardStats)
				.Include(u => u.CareerPath)
				.FirstOrDefaultAsync(u => u.Id == userId);

			if (user == null) return;

			// 2. تحديث بيانات اليوزر الأساسية
			user.Headline = headline;
			user.ExperienceYears = exp;

			// 3. تحديث أو إضافة DashboardStats
			if (user.DashboardStats == null)
			{
				user.DashboardStats = new UserDashboardStats
				{
					UserId = userId,
					MarketValue = marketValue,
					SkillRank = "N/A",
					ProfileViews = 0
				};
				// EF Core رح يفهم إنه لازم يضيفه لأنه مرتبط بالـ user اللي حصله Include
			}
			else
			{
				user.DashboardStats.MarketValue = marketValue;
			}

			// 4. تحديث أو إضافة CareerPath
			if (user.CareerPath == null)
			{
				user.CareerPath = new CareerArchitect
				{
					UserId = userId,
					AIRecommendation = recommendation,
					ProgressPct = progres,
					TargetTitle = targetTitle,
					// ✅ التعديل هون: خزن الـ string مباشرة بدون Serialize إضافي
					RoadmapJson = roadMap
				};
			}
			else
			{
				user.CareerPath.AIRecommendation = recommendation;
				user.CareerPath.ProgressPct = progres;
				user.CareerPath.TargetTitle = targetTitle;
				// ✅ التعديل هون: تحديث الـ JSON مباشرة
				user.CareerPath.RoadmapJson = roadMap;
			}

			// 5. حفظ التغييرات
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating dashboard data for user {UserId}", userId);
				throw;
			}
		}
	}
}

