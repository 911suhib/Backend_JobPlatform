using JobPlatformBackend.Contracts.Contracts.Jop;
using JobPlatformBackend.Contracts.Contracts.Jop.Create;
using JobPlatformBackend.Contracts.Contracts.Jop.Get;
using JobPlatformBackend.Contracts.Contracts.Shared;
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
		Task<PagedResponseDto<JobResponseDto>> GetAllJobsByCompanyIdAsync(GetByCompanyIdDto getByCompanyId);
		Task<PagedResponseDto<JobResponseDto>> GetAllJobsBySkillNameAsync(GetBySkillNameDto getBySkill);
		Task<JobResponseDto> GetJobById(int id);
	}
}
