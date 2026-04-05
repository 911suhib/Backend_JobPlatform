using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.Company.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobPlatformBackend.API.Controllers
{
	[ApiController]
	[Route("api/v1/company")]
	public class CompanyController:ControllerBase
	{
		private readonly ICompanyService _companyService;
		public CompanyController(ICompanyService companyService)
		{
			_companyService = companyService;
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyRequest request)
		{
			var user = User.FindFirst(ClaimTypes.NameIdentifier);
			if (user == null) return Unauthorized();

			int userId= int.Parse(user.Value);

			await _companyService.CreateCompanyAsync(request,userId);
			return Ok(new { message = "Company created successfully with you as the owner." });
		}	
	}
}
