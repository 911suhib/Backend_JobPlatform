using JobPlatformBackend.Domain.src.Entity;
using Microsoft.EntityFrameworkCore.Storage;

namespace JobPlatformBackend.Domain.src.Abstractions
{
	public interface ICompanyRepository : IBaseRepository<Company> {
		Task AddAdminToCompanyAsync(CompanyAdmin companyAdmin);
		Task<IDbContextTransaction> BeginTransactionAsync();
		Task<Company> CreateCompanyAsync(Company company);
		Task<bool> IsUserAdminOfCompanyAsync(int userId, int companyId);
	}
}
