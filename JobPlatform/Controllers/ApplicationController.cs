using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.Application.Get;
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
	}
}
