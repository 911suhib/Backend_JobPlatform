using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.Application;
using JobPlatformBackend.Contracts.Contracts.Application.Create;
using JobPlatformBackend.Contracts.Contracts.Application.Get;
using JobPlatformBackend.Contracts.Contracts.Shared;
using JobPlatformBackend.Domain.src.Abstractions;
using JobPlatformBackend.Domain.src.Entity;
using JobPlatformBackend.Domain.src.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Implementations
{
	public class ApplicationService : IApplicationService
	{
		private readonly IApplicationRepository _applicationRepository;
		private readonly ICompanyRepository _companyRepository;
		private readonly IJobRepository _jobRepository;
		private readonly IFileService _fileService;

		public ApplicationService(IApplicationRepository application, ICompanyRepository companyRepository, IJobRepository jobRepository, IFileService fileService)
		{
			_applicationRepository = application;
			_companyRepository = companyRepository;
			_jobRepository = jobRepository;
			_fileService = fileService;
		}

		public async Task<bool> ApplyToJobAsync(int userId, int jobId, IFormFile cvFile)
		{
			var applied = await _applicationRepository.GetByUserIdAndJobIdAsync(userId, jobId);
			if (applied) throw new BadRequestException("You have already applied to this job.");
			if (cvFile == null || cvFile.Length == 0) throw new BadRequestException("CV file is required.");
			if (cvFile.Length > 5 * 1024 * 1024) throw new BadRequestException("CV file size should not exceed 5MB.");
			var allowedExtensions = new[] { ".pdf", ".docx", ".doc" };
			var extension = Path.GetExtension(cvFile.FileName).ToLower();
			if (!allowedExtensions.Contains(extension))
				throw new BadRequestException("Only PDF and Word documents are allowed.");
			var (url, publicId) = await _fileService.UploadAsync(cvFile, "JobPlatform/Resumes");


			var application = new Application
			{
				UserId = userId,
				JobId = jobId,
				CvUrl = url,
				CvPublicId = publicId,
				CreatedAt = DateTime.UtcNow
			};
			await _applicationRepository.AddAsync(application);
			await _applicationRepository.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteApplicationAsync(int userId, int applicationId)
		{
			var application = await _applicationRepository.GetByIdAsync(applicationId);
			if (application == null) throw new NotFoundException("Application not found.");
			if (application.UserId != userId) throw new ForbiddenException("You don't have permission to delete this application.");
			await _fileService.DeleteAsync(application.CvPublicId);
			await _applicationRepository.DeleteAsync(application);
			await _applicationRepository.SaveChangesAsync();
			return true;
		}

		public async Task<PagedResponseDto<ApplicationResponse>> GetApplicationsByJobIdAsync(int userId, GetAllApplicationRequest request)
		{
			var jobExists = await _jobRepository.JobExistsAsync(request.JobId, request.CompanyId);
			var isAdmin = await _companyRepository.IsUserAdminOfCompanyAsync(request.CompanyId, userId);

			if (!isAdmin) throw new ForbiddenException("You don't have permission to access this company's data.");

			if (!jobExists)
			{
				throw new NotFoundException("this job does not exist");
			}
			var applications = await _applicationRepository.GetByJobIdAsync(request.JobId, request.PageNumber, request.PageSize);

			return new PagedResponseDto<ApplicationResponse>
			{
				Items = applications,
				PageNumber = request.PageNumber,
				PageSize = request.PageSize,
				TotalCount = await _applicationRepository.GetCountByJobIdAsync(request.JobId)
			}
			;
		}


		public async Task<bool> UpdateStatusAsync(int userId, int applicationId, string status)
		{
			if (!Enum.TryParse<StatusApplication>(status, true, out var parsedStatus))
			{
				throw new BadRequestException($"Invalid status value: {status}");
			}

			var applicaiton = await _applicationRepository.UpdateStatusAsync(applicationId,parsedStatus);
 
			var isAdmin = await _companyRepository.IsUserAdminOfCompanyAsync(applicaiton.Job.CompanyId, userId);
			if (!isAdmin) throw new ForbiddenException("No permission to update this application.");

 			await _applicationRepository.SaveChangesAsync();
			return true;
		}
		public async Task<bool> WithdrawApplicationAsync(int userId, int applicationId)
		{

			var application = await _applicationRepository.GetByIdAsync(applicationId);

			if (application == null) throw new NotFoundException("Application not found.");

			if (application.UserId != userId)
				throw new ForbiddenException("You can only withdraw your own applications.");


			if (application.Status != StatusApplication.Pending)
				throw new BadRequestException("You cannot withdraw an application that has already been processed.");

			await _applicationRepository.UpdateStatusAsync(applicationId, StatusApplication.Withdrawn);

			await _applicationRepository.SaveChangesAsync();

			return true;
		}

		public async Task<PagedResponseDto<MyApplicationResponse>> GetApplicationByIdAsync(int userId, int pageNumber, int pageSize)
		{
			pageNumber = pageNumber < 1 ? 1 : pageNumber;
			const int maxPageSize = 50;
			pageSize = pageSize < 1 ? 10 : (pageSize > maxPageSize ? maxPageSize : pageSize);
			var applications = await _applicationRepository.GetByUserIdAsync(userId, pageNumber, pageSize);
			var totalCount = await _applicationRepository.CountAsync(a => a.UserId == userId);
			if (applications == null || !applications.Any()) throw new NotFoundException("Application not found.");

			var response = applications.Select(app => new MyApplicationResponse(
				app.Id,
				app.Job?.Title ?? "N/A",           // استخدم ? لضمان عدم الانهيار
				app.Job?.Company?.Name ?? "N/A",   // إذا كان الـ Job أو Company نل، سيعرض N/A
				app.Job?.Company?.LogoUrl ?? "",
				app.Status.ToString(),
				app.CreatedAt,
				app.Job?.Location ?? "Unknown",
				app.JobId
			)).ToList(); 
			return new PagedResponseDto<MyApplicationResponse>
			{
				Items = response,
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalCount = totalCount,
				
			};
		}
		public async Task<ApplicationResponse> GetApplicationDetailsAsync(int userId, int applicationId)
		{
 			var data = await _applicationRepository.Query()
				.Where(x => x.Id == applicationId)
				.Select(app => new
				{
					Response = new ApplicationResponse(
						app.Id,
						app.UserId,
						app.User.FName + " " + app.User.LName,
						app.User.Email,
						app.JobId,
						app.Status.ToString(),
						app.Job.Title,
						app.CreatedAt,
						app.CvUrl
					),
					CompanyId = app.Job.CompanyId  
				})
				.FirstOrDefaultAsync();

			if (data == null) throw new NotFoundException("Application not found.");

 			var isAdmin = await _companyRepository.IsUserAdminOfCompanyAsync(data.CompanyId, userId);

			if (!isAdmin && data.Response.UserId != userId)
				throw new ForbiddenException("You don't have permission to access this application.");

			return data.Response;
		}

	}

}
