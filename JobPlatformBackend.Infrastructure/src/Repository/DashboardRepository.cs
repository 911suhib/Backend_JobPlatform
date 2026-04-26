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
		public async Task UpdateUserDashboardDataAsync(int userId, string headline, string exp, string marketValue, string recommendation ,int progres,string targetTitle,List<SkillGap> missingSkills)
		{
			// 1. جلب اليوزر الأساسي
			var user = await _context.Users
				.Include(u => u.DashboardStats)
				.Include(u => u.CareerPath)
				.FirstOrDefaultAsync(u => u.Id == userId);

			if (user == null) return;

			// 2. تحديث بيانات اليوزر
			user.Headline = headline;
			user.ExperienceYears = exp;

			// 3. تحديث أو إضافة DashboardStats
			if (user.DashboardStats == null)
			{
				// الجدول فاضي؟ إذاً ننشئ سطر جديد ونربطه بالـ UserId
				var newStats = new UserDashboardStats
				{
					UserId = userId,
					MarketValue = marketValue,
					SkillRank = "N/A", // قيم افتراضية
					ProfileViews = 0
				};
				_context.UserDashboardStats.Add(newStats);
			}
			else
			{
				user.DashboardStats.MarketValue = marketValue;
			}

			// 4. تحديث أو إضافة CareerPath
			if (user.CareerPath == null)
			{
				// الجدول فاضي؟ ننشئ سطر جديد
				var newPath = new CareerArchitect
				{
					UserId = userId,
					AIRecommendation = recommendation,
					ProgressPct = progres,
					TargetTitle = targetTitle,
					RoadmapJson = JsonConvert.SerializeObject(missingSkills)
				};
				_context.CareerArchitects.Add(newPath);
			}
			else
			{
				user.CareerPath.AIRecommendation = recommendation;
				user.CareerPath.ProgressPct = progres;
				user.CareerPath.TargetTitle=targetTitle;
			}
			if (missingSkills != null)
				user.CareerPath.RoadmapJson = JsonConvert.SerializeObject(missingSkills);

			// 5. حفظ التغييرات
			await _context.SaveChangesAsync();
		}
	}
}

