using JobPlatformBackend.Contracts.Contracts.Jop;
using JobPlatformBackend.Domain.src.Entity;

namespace JobPlatformBackend.Domain.src.Abstractions
{
	public interface IJobRepository : IBaseRepository<Job>
	{
		Task<(IEnumerable<JobResponseDto> Items, int TotalCount)> GetByCompanyIdAsync(int companyId, int page, int pageSize);
 		Task<Job?> GetWithDetailsAsync(int jobId);

		Task<(IEnumerable<JobResponseDto> Items, int TotalCount)> GetAllBySkillNameAsync(int page, int pageSize,string skill);

		Task<IEnumerable<JobResponseDto>> SearchAsync(string? title = null,string? location = null,string? jobType = null,int? companyId = null,string? skill = null,int page = 1,int pageSize = 10);

		Task<IEnumerable<JobResponseDto>> GetJobsForUserAsync(int userId,int page=1 ,int pageSize=15);
		Task<Job?> GetJobWithId(int id);
		Task<List<Skill>> GetByNamesAsync(List<string> names);
		Task<bool> JobExistsAsync(int jobId, int companyId);
	}
}
