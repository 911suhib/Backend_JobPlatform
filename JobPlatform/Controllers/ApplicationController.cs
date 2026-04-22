using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.Application.Create;
using JobPlatformBackend.Contracts.Contracts.Application.Get;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPlatformBackend.API.Controllers
{
	[ApiController]
	[Route("api/v1/application")]
	public class ApplicationController:ControllerBase
	{
		private readonly IApplicationService _applicationService;
		public ApplicationController(IApplicationService applicationService) { 
		
			_applicationService = applicationService;
		}


		[HttpPost("job/applications")]
		public async Task<IActionResult> GetApplicationsByJobId(int userId,[FromBody] GetAllApplicationRequest request)
		{
 			var applications = await _applicationService.GetApplicationsByJobIdAsync(userId, request);
			return Ok(applications);
		}
		[HttpPost("apply")]
 		public async Task<IActionResult> ApplyToJob( int userId,int jobId,IFormFile cvFile)
		{
			//var userid=User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
			var result = await _applicationService.ApplyToJobAsync(userId, jobId, cvFile);
			if (!result) return BadRequest();
			return Ok(new { Message = "Application submitted successfully." });
		}
	}
}
