using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.Company.Create;
using JobPlatformBackend.Contracts.Contracts.User.Shared;
using JobPlatformBackend.Domain.src.Abstractions;
using JobPlatformBackend.Domain.src.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Implementations
{
	public class CompanyService : ICompanyService
	{
		private readonly ICompanyRepository _companyRepository;
		private readonly ISanitizerService _sanitizerService;
		public CompanyService(ICompanyRepository companyRepository,ISanitizerService sanitizerService)
		{
			_companyRepository = companyRepository;
			_sanitizerService = sanitizerService;
		}
		public Task AddAdminToCompanyAsync(int companyId, int userId)
		{
			throw new NotImplementedException();
		}

		public async Task CreateCompanyAsync(CreateCompanyRequest request,int userId)
		{
			var sanitizedDto = _sanitizerService.SanitizeDto(request);
			var IsValidEmail = Validator.IsValidEmail(sanitizedDto.Email);
			if (!IsValidEmail)
			{
				throw new ArgumentException("Invalid Email address.");
			}
		using var transaction =await _companyRepository.BeginTransactionAsync();
			try
			{
				var companyEntity = new Company
				{
					Name = sanitizedDto.Name,
					Descriptoin = sanitizedDto.Description,
					Email = sanitizedDto.Email,
					Location = sanitizedDto.Location,
					LogoUrl = sanitizedDto.LogoUrl,
					CreatedAt= DateTime.UtcNow,
					
				};
				var createdCompany =await _companyRepository.CreateCompanyAsync(companyEntity);

				var companyAdminEntity = new CompanyAdmin
				{
					CompanyId = createdCompany.Id,
					UserId = userId,
 					Role = RoleCompany.Owner,
					AssignedAt=DateTime.UtcNow,
				};
				await _companyRepository.AddAdminToCompanyAsync(companyAdminEntity);
				await transaction.CommitAsync();

			}
			catch (Exception)
			{
			 await	transaction.RollbackAsync();
				throw;
			}
		}

		public Task DeleteCompanyAsync(int companyId)
		{
			throw new NotImplementedException();
		}

		public Task RemoveAdminFromCompanyAsync(int companyId, int userId)
		{
			throw new NotImplementedException();
		}

		public Task UpdateCompanyAsync(CreateCompanyRequest request)
		{
			throw new NotImplementedException();
		}
	}
}
