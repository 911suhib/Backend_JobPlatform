using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.Jop;
using JobPlatformBackend.Contracts.Contracts.Jop.Create;
using JobPlatformBackend.Contracts.Contracts.Jop.Get;
using JobPlatformBackend.Infrastructure.src.Repository;
using Microsoft.AspNetCore.Mvc;

namespace JobPlatformBackend.API.Controllers
{
	[ApiController]
	[Route("api/v1/job")]
	public class JobController : ControllerBase
	{
		private readonly IJobService _jobService;

		public JobController(IJobService jobService)
		{
			_jobService = jobService;
		}

		[HttpPost]
		public async Task<ActionResult<JobResponseDto>> CreateJob(CreateJobRequest request, int userId)
		{
			var result = await _jobService.CreatJobAsync(request, userId);
			return Ok(
				result
				);
		}

		[HttpGet("search-by-skill")]
		public async Task<ActionResult<JobResponseDto>> GetJobSkill([FromQuery] GetBySkillNameDto searchDto)
		{
			if (string.IsNullOrWhiteSpace(searchDto.skill))
			{
				return BadRequest(new { Message = "Please enter name before search" });
			}
			var result = await _jobService.GetAllJobsBySkillNameAsync(searchDto);

			return Ok(result);
		}

		[HttpGet("company")]
		public async Task<ActionResult<JobResponseDto>> GetJobByCompanyId([FromQuery] GetByCompanyIdDto searchDto)
		{
			var result = await _jobService.GetAllJobsByCompanyIdAsync(searchDto);
			return Ok(result);
		}
		[HttpGet]
		public async Task<ActionResult<JobResponseDto>> GetJobById([FromQuery] int id)
		{
			var result = await _jobService.GetJobById(id);
			return Ok(result);
		}
	}
}
