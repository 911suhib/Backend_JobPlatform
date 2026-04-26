using JobPlatformBackend.API.Contracts.User.GetAll;

namespace JobPlatformBackend.Business.src.Services.Abstractions
{
	public interface IDashboardService
	{
		Task<UserDashboardDto> GetUserDashboardAsync(int userId);
		Task<bool> InitializeDashboardAsync(int userId); // لإنشاء سجلات فارغة عند التسجيل
	}
}
