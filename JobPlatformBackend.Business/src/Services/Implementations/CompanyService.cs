using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.Company.Create;
using JobPlatformBackend.Contracts.Contracts.Company.Get;
using JobPlatformBackend.Contracts.Contracts.Company.Update;
using JobPlatformBackend.Contracts.Contracts.Shared;
using JobPlatformBackend.Contracts.Contracts.User.Shared;
using JobPlatformBackend.Domain.src.Abstractions;
using JobPlatformBackend.Domain.src.Entity;
using JobPlatformBackend.Domain.src.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Implementations
{
	public class CompanyService : ICompanyService
	{
		private readonly ICompanyRepository _companyRepository;
		private readonly IImageService _imageService;
		private readonly ISanitizerService _sanitizerService;
		private readonly BaseService<Company, CompanyResponse> _base;
		public CompanyService(ICompanyRepository companyRepository,IImageService imageService,ISanitizerService sanitizerService, BaseService<Company, CompanyResponse> baseService)
		{
			_companyRepository = companyRepository;
			_sanitizerService = sanitizerService;
			_imageService = imageService;
			_base = baseService;
		}
		public async Task AddAdminToCompanyAsync(CreateNewAdmin createNew,int userId)
		{

			var user = await _companyRepository.GetOwnerAsync(userId, createNew.CompanyId);
 

			if (!user) throw new Exception("User is not the owner of the company.");
			var newAdmin = new CompanyAdmin
			{
				CompanyId = createNew.CompanyId,
				UserId = createNew.UserId,
				Role = RoleCompany.Admin,
				AssignedAt = DateTime.UtcNow,
			};
						await _companyRepository.AddAdminToCompanyAsync(newAdmin);
		}

		public async Task<CreateCompanyResponse> CreateCompanyAsync(CreateCompanyRequest request, int userId)
		{
			var sanitizedDto = _sanitizerService.SanitizeDto(request);
			var IsValidEmail = Validator.IsValidEmail(sanitizedDto.Email);
			if (!IsValidEmail)
			{
				throw new ArgumentException("Invalid Email address.");
			}
			using var transaction = await _companyRepository.BeginTransactionAsync();
			try
			{
				var companyEntity = new Company
				{
					Name = sanitizedDto.Name,
					Descriptoin = sanitizedDto.Description,
					Email = sanitizedDto.Email,
					Location = sanitizedDto.Location,
					LogoUrl = sanitizedDto.LogoUrl,
					CreatedAt = DateTime.UtcNow,

				};
				var createdCompany = await _companyRepository.CreateCompanyAsync(companyEntity);

				var companyAdminEntity = new CompanyAdmin
				{
					CompanyId = createdCompany.Id,
					UserId = userId,
					Role = RoleCompany.Owner,
					AssignedAt = DateTime.UtcNow,
				};
				await _companyRepository.AddAdminToCompanyAsync(companyAdminEntity);
				await transaction.CommitAsync();
				return new CreateCompanyResponse(createdCompany.Id, createdCompany.Name, createdCompany.Email,createdCompany.Descriptoin, createdCompany.Location, createdCompany.LogoUrl);

			}
			catch (Exception)
			{
				await transaction.RollbackAsync();
				throw;
			}
		}
		public async Task<string> UpdateLogoUrlCompany(IFormFile file,int companyId,int userId)
		{
			var company = await _companyRepository.GetByIdAsync(companyId);
			if (company == null) throw new Exception("Company not found");

			var isAdmin = await _companyRepository.IsUserAdminOfCompanyAsync(companyId, userId);
			if (!isAdmin) throw new UnauthorizedAccessException();

			var (url, publicId) = await _imageService.ReplaceAsync(file, "JobPlatform/Companies", company.ProfileImagePublicId);
			company.LogoUrl = url;
			company.ProfileImagePublicId = publicId;
			await _companyRepository.UpdateAsync(company);
			await _companyRepository.SaveChangesAsync();
			return url;
		}

		public async Task DeleteCompanyAsync(int companyId,int userId)
		{
			var isAdmin = await _companyRepository.IsUserAdminOfCompanyAsync(companyId, userId);
			if (!isAdmin) throw new UnauthorizedAccessException();

			var company = await _companyRepository.GetByIdAsync(companyId);	
			if (company is null)
			{
				throw new BadRequestException("Company not found.");
			}

			await _companyRepository.DeleteAsync(company); 
		}

		public async Task RemoveAdminFromCompanyAsync(int companyId, int userId,int userDelete)
		{
			var user=await _companyRepository.GetOwnerAsync(userId, companyId);

			if (!user) throw new UnauthorizedAccessException("User is not the owner of the company.");

			try
			{
				await _companyRepository.RemoveAdminFromCompanyAsync(companyId, userId);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Failed to remove admin from company.", ex);
			}

			}

		public async Task UpdateCompanyAsync(UpdateCompanyRequest request, int userId)
		{
			var company = await _companyRepository.GetByIdAsync(request.companyId);
			if (company is null)
			{
				throw new BadRequestException("Company not found.");
			}
			var isAdmin =  await _companyRepository.IsUserAdminOfCompanyAsync(request.companyId, userId);

			if (!isAdmin) throw new UnauthorizedAccessException("User is not the owner of the company.");

			var sanitizedDto = _sanitizerService.SanitizeDto(request);

			 company.Name = sanitizedDto.Name ?? company.Name;
			company.Descriptoin = sanitizedDto.Description ?? company.Descriptoin;
			company.Location = sanitizedDto.Location ?? company.Location;

			try
			{
				await _companyRepository.UpdateAsync(company);
				await _companyRepository.SaveChangesAsync();
			}
			catch (Exception ex)
			{
 				throw new Exception("An error occurred while saving the company updates.", ex);
			}
		}

		public async Task<IEnumerable<CompanyResponse>> GetCompaniesByOwnerAsync(int userId)
		{
			var companies = await _companyRepository.GetCompaniesByUserIdAsync(userId);

			// تحويل الـ Entities إلى الـ Response DTO الذي أنشأناه فوق
			return companies.Select(c => new CompanyResponse(
				c.Id,
				c.Name,
				c.Descriptoin, // السبلنج الخاص بك
				c.Location,
				c.LogoUrl
			));
		}
		public async Task<CompanyResponse> GetCompanyByIdAsync(int id)
		{
			var company = await _companyRepository.GetByIdAsync(id);
			if (company == null) return null;

			return new CompanyResponse(company.Id, company.Name, company.Descriptoin, company.Location, company.LogoUrl);
		}

		public async Task<IEnumerable<AdminResponse>> GetAdminsByCompanyIdAsync(int companyId, int currentUserId)
		{
			// تأكد أولاً أن currentUserId هو Owner لهذه الشركة
			var isOwner = await _companyRepository.GetOwnerAsync(currentUserId, companyId);
			if (!isOwner) throw new UnauthorizedAccessException();

			var admins = await _companyRepository.GetAdminsAsync(companyId);
			return admins.Select(a => new AdminResponse(a.UserId, a.User.FName + " " + a.User.LName, a.Role.ToString()));
		}

		public async Task<PagedResponseDto<CompanyResponse>> GetAllCompaniesAsync(QueryOptions options)
		{
			Expression<Func<Company,CompanyResponse>> mapper= company=>new CompanyResponse
			(
				company.Id,
				company.Name,
				company.Descriptoin,
				company.Location,
				company.LogoUrl
			);
			return await _base.GetAll(options, mapper);
		}
	}
}
