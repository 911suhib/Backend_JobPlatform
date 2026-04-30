using JobPlatformBackend.Contracts.Contracts.Company.Create;
using JobPlatformBackend.Contracts.Contracts.Company.Get;
using JobPlatformBackend.Contracts.Contracts.Company.Update;
using JobPlatformBackend.Contracts.Contracts.Shared;
using JobPlatformBackend.Domain.src.Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Abstractions
{
	public interface ICompanyService
	{
		Task <CreateCompanyResponse>CreateCompanyAsync(CreateCompanyRequest request,int userId);
		 Task UpdateCompanyAsync(UpdateCompanyRequest request, int userId);
		 Task DeleteCompanyAsync(int companyId, int userId);
		 Task AddAdminToCompanyAsync(CreateNewAdmin createNew,int userId);
		 Task RemoveAdminFromCompanyAsync(int companyId, int userId, int userDelete);
		Task<string> UpdateLogoUrlCompany(IFormFile file, int companyId, int userId);
		Task<IEnumerable<CompanyResponse>> GetCompaniesByOwnerAsync(int userId);
		Task<CompanyResponse> GetCompanyByIdAsync(int id);
		Task<IEnumerable<AdminResponse>> GetAdminsByCompanyIdAsync(int companyId, int currentUserId);

		Task <PagedResponseDto<CompanyResponse>> GetAllCompaniesAsync(QueryOptions options);
 	}
}
