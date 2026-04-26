using JobPlatformBackend.Domain.src.Entity;
using Microsoft.EntityFrameworkCore.Storage;

namespace JobPlatformBackend.Domain.src.Abstractions
{
	public interface ICompanyRepository : IBaseRepository<Company> {
		Task AddAdminToCompanyAsync(CompanyAdmin companyAdmin);
		Task<IDbContextTransaction> BeginTransactionAsync();
		Task<Company> CreateCompanyAsync(Company company);
		Task<IEnumerable<CompanyAdmin>> GetAdminsAsync(int companyId);
 		Task<IEnumerable<Company>> GetCompaniesByUserIdAsync(int userId);
		Task<bool> GetOwnerAsync(int userId, int compnyId);
		Task<bool> IsUserAdminOfCompanyAsync( int companyId, int userId);
		Task RemoveAdminFromCompanyAsync(int companyId, int userId);
	}
	
}
