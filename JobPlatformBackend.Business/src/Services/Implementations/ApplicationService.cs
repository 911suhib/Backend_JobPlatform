using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.Application;
using JobPlatformBackend.Contracts.Contracts.Application.Get;
using JobPlatformBackend.Contracts.Contracts.Shared;
using JobPlatformBackend.Domain.src.Abstractions;
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

		public ApplicationService(IApplicationRepository application,ICompanyRepository companyRepository,IJobRepository jobRepository)
		{
			_applicationRepository = application;
			_companyRepository = companyRepository;
			_jobRepository = jobRepository;

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
