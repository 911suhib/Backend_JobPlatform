using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.Jop;
using JobPlatformBackend.Contracts.Contracts.Jop.Create;
using JobPlatformBackend.Contracts.Contracts.Jop.Get;
using JobPlatformBackend.Contracts.Contracts.Jop.Update;
using JobPlatformBackend.Infrastructure.src.Repository;
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

		[HttpDelete("{id}")]
		
		public async Task<IActionResult> DeleteJob(int id,int adminId)
		{
 			await _jobService.DeleteJobAsync(id, adminId);

			return NoContent(); // الـ Status Code 204 هو "الرسالة" التقنية للنجاح
		}

		[HttpPut("{jobId:int}")]
		public async Task<IActionResult> EditJob(int adminId, int jobId, [FromBody] UpdateRequestDto request)
		{
		 
 

 
			// 2. استدعاء الـ Service
			// الـ Service هي اللي بتتحقق من الصلاحيات وجودة البيانات
			await _jobService.EditJobAsync( adminId, jobId,request);

			// 3. الإرجاع الناجح
			// في الـ Update، من الأفضل إرجاع NoContent (204) أو Ok مع رسالة
			return Ok(new { Message = "تم تحديث بيانات الوظيفة والمهارات بنجاح" });
		}
	}
}
