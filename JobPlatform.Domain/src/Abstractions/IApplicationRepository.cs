using JobPlatformBackend.Contracts.Contracts.Application;
using JobPlatformBackend.Domain.src.Entity;

namespace JobPlatformBackend.Domain.src.Abstractions
{
	public interface IApplicationRepository : IBaseRepository<Application> { 
	
		// get all applications for a specific job
	Task <IEnumerable<ApplicationResponse>> GetByJobIdAsync(int jobId, int pageNumber, int pageSize);

		//get all applications by a specific user
	Task<IEnumerable<Application>> GetByUserIdAsync();
		//update application status
	Task<Application>UpdateStatusAsync(int applicationId,StatusApplication status);

		// Optional: get application with Job and User details
		Task<Application> GetWithDetailsAsync(int applicationId);
		Task<int> GetCountByJobIdAsync(int jobId);
	}
}
