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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Implementations
{
	public class ApplicationService:IApplicationService
	{
		private readonly IApplicationRepository _applicationRepository;	
		private readonly ICompanyRepository _companyRepository;
		private readonly IJobRepository _jobRepository;
		private readonly IFileService _fileService;

		public ApplicationService(IApplicationRepository application,ICompanyRepository companyRepository,IJobRepository jobRepository,IFileService fileService)
		{
			_applicationRepository = application;
			_companyRepository = companyRepository;
			_jobRepository = jobRepository;
			_fileService = fileService;
		}

		public async Task<bool> ApplyToJobAsync(int userId, int jobId , IFormFile cvFile)
		{
			var applied = await _applicationRepository.GetByUserIdAndJobIdAsync(userId, jobId);
			if (applied) throw new BadRequestException("You have already applied to this job.");
			if (cvFile == null || cvFile.Length == 0) throw new BadRequestException("CV file is required.");
			if (cvFile.Length > 5 * 1024 * 1024) throw new BadRequestException("CV file size should not exceed 5MB.");
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
			var isAdmin = await _companyRepository.IsUserAdminOfCompanyAsync( request.CompanyId,userId);

			if (!isAdmin) throw new ForbiddenException("You don't have permission to access this company's data.");

			if (!jobExists) { 
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

	 
	}
}
