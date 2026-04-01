using JobPlatformBackend.Contracts.Contracts.Jop;
using JobPlatformBackend.Contracts.Contracts.Jop.Create;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Abstractions
{
	public interface IJobService
	{
		Task<JobResponseDto> CreatJobAsync(CreateJobRequest request,int userId);
	}
}
