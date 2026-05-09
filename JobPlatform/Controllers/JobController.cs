using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.Jop;
using JobPlatformBackend.Contracts.Contracts.Jop.Create;
using JobPlatformBackend.Contracts.Contracts.Jop.Get;
using JobPlatformBackend.Contracts.Contracts.Jop.Update;
using JobPlatformBackend.Domain.src.Entity;
using JobPlatformBackend.Infrastructure.src.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
		[Authorize]
		public async Task<ActionResult<JobResponseDto>> CreateJob(CreateJobRequest request)
		{
			int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

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

		[HttpDelete("{id}")]
		[Authorize]
		
		public async Task<IActionResult> DeleteJob(int id)
		{
			int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));


			await _jobService.DeleteJobAsync( id ,userId);

			return NoContent(); 
		}

		[HttpPut("{jobId:int}")]
		public async Task<IActionResult> EditJob( int jobId, [FromBody] UpdateRequestDto request)
		{

			int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

			await _jobService.EditJobAsync(  jobId, userId, request);
 
			return Ok(new { Message = "تم تحديث بيانات الوظيفة والمهارات بنجاح" });
		}
	}
}
