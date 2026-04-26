using JobPlatformBackend.Contracts.Contracts.AI;
using JobPlatformBackend.Domain.src.Entity;

namespace JobPlatformBackend.Domain.src.Abstractions
{
	public interface IDashboardRepository : IBaseRepository<User>
	{
		// ميثود إضافية مخصصة فقط للداشبورد (Eager Loading)
		Task<User?> GetFullDashboardDataAsync(int userId, CancellationToken cancellationToken = default);
		Task UpdateUserDashboardDataAsync(int userId, string headline, string exp, string marketValue, string recommendation, int progres, string targetTitle, List<SkillGap> missingSkills);
	}
}
