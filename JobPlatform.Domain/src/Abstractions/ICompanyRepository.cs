using JobPlatformBackend.Domain.src.Entity;

namespace JobPlatformBackend.Domain.src.Abstractions
{
	public interface ICompanyRepository : IBaseRepository<Company> {
		Task<bool> IsUserAdminOfCompanyAsync(int userId, int companyId);
	}
}
