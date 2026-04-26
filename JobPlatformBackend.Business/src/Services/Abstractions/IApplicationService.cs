using JobPlatformBackend.Contracts.Contracts.Application;
using JobPlatformBackend.Contracts.Contracts.Application.Create;
using JobPlatformBackend.Contracts.Contracts.Application.Get;
using JobPlatformBackend.Contracts.Contracts.Shared;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Abstractions
{

	public interface IApplicationService
	{
		Task<PagedResponseDto<ApplicationResponse>> GetApplicationsByJobIdAsync(int userId, GetAllApplicationRequest request);

		Task<bool> ApplyToJobAsync(int userId,int jobId, IFormFile cvFile);
		Task<bool> DeleteApplicationAsync(int userId, int applicationId);
		Task<bool> UpdateStatusAsync(int userId, int applicationId, string status);
		Task<bool> WithdrawApplicationAsync(int userId, int applicationId);
		Task<PagedResponseDto<MyApplicationResponse>> GetApplicationByIdAsync(int userId, int pageNumber, int pageSize);
		Task<ApplicationResponse> GetApplicationDetailsAsync(int userId, int applicationId);
	}

}
