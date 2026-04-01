using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.Jop.Create;
using JobPlatformBackend.Infrastructure.src.Repository;
using Microsoft.AspNetCore.Mvc;

namespace JobPlatformBackend.API.Controllers
{
	[ApiController]
	[Route("api/v1/job")]
	public class JobController:ControllerBase
	{
		private readonly IJobService _jobService;

		public JobController(IJobService jobService)
		{
			_jobService = jobService;
		}

		[HttpPost]
		public async Task<ActionResult<JobRepository>> CreateJop(CreateJobRequest request, [FromBody] int userId)
		{
			var result =await _jobService.CreatJobAsync(request,userId);
			return Ok(
				result
				);
		}
	}
}
