using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.Company.Create;
using JobPlatformBackend.Contracts.Contracts.Company.Get;
using JobPlatformBackend.Contracts.Contracts.Company.Update;
using JobPlatformBackend.Contracts.Contracts.Shared;
using JobPlatformBackend.Domain.src.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobPlatformBackend.API.Controllers
{
	[ApiController]
	[Route("api/v1/company")]
	public class CompanyController : ControllerBase
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

			int userId = int.Parse(user.Value);

			var company=	await _companyService.CreateCompanyAsync(request, userId);
			return Ok(company);
 		}
		[HttpPost("{companyId}/logo")]
		[Authorize]
		public async Task<IActionResult> UploadLogo(int companyId, IFormFile file)
		{
			var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
			var imageUrl = await _companyService.UpdateLogoUrlCompany(file, companyId, userId);
			return Ok(new { imageUrl = imageUrl });
		}
		[HttpPost("admins")]
		[Authorize]

		public async Task<IActionResult> AddAdminToCompany(CreateNewAdmin createNew)
		{
			var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
			await _companyService.AddAdminToCompanyAsync(createNew, currentUserId);
			return Ok(new { message = "Admin added to company successfully." });
		}

		[HttpGet("my-companies")]
		[Authorize]
		public async Task<IActionResult> GetMyCompanies()
		{
			var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
			var companies = await _companyService.GetCompaniesByOwnerAsync(userId);
			return Ok(companies);
		}
		[HttpGet("{id}")]
		[AllowAnonymous]
		public async Task<IActionResult> GetCompanyById(int id)
		{
			var company = await _companyService.GetCompanyByIdAsync(id);
			if (company == null) return NotFound("Company not found.");
			return Ok(company);
		}

		[HttpPut("update")]
		[Authorize]
		public async Task<IActionResult> UpdateCompany([FromBody] UpdateCompanyRequest request)
		{
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
			if (userIdClaim == null)
			{
				return Unauthorized("User ID not found in token.");
			}

			int userId = int.Parse(userIdClaim.Value);

			try
			{
				await _companyService.UpdateCompanyAsync(request, userId);
				return Ok(new { message = "Company updated successfully." });
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (UnauthorizedAccessException ex)
			{
				// إذا لم يكن المستخدم أدمن أو مالك
				return Forbid();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}


		[HttpDelete("{companyId}")]
		[Authorize]
		public async Task<IActionResult> DeleteCompany(int companyId)
		{
			var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

			try
			{
				await _companyService.DeleteCompanyAsync(companyId, userId);
				return Ok(new { message = "Company deleted successfully (Soft Delete)." });
			}
			catch (UnauthorizedAccessException) { return Forbid(); }
			catch (BadRequestException ex) { return NotFound(ex.Message); }
		}

		[HttpGet("{companyId}/admins")]
		[Authorize]
		public async Task<IActionResult> GetCompanyAdmins(int companyId)
		{
			var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
			try
			{
				var admins = await _companyService.GetAdminsByCompanyIdAsync(companyId, userId);
				return Ok(admins);
			}
			catch (UnauthorizedAccessException) { return Forbid(); }
			catch (Exception ex) { return BadRequest(ex.Message); }
		}
		[HttpDelete("{companyId}/admins/{adminId}")]
		[Authorize]
		public async Task<IActionResult> RemoveAdmin(int companyId, int adminId)
		{
			var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
			try
			{
				await _companyService.RemoveAdminFromCompanyAsync(companyId, adminId, currentUserId);
				return Ok(new { message = "Admin removed successfully." });
			}
			catch (UnauthorizedAccessException) { return Forbid(); }
			catch (Exception ex) { return BadRequest(ex.Message); }
		}
		[HttpPost("companies")]
		public async Task <ActionResult> GetAllCompany(QueryOptions query)
		{
		  return  Ok (await	_companyService.GetAllCompaniesAsync(query));
		}


	}


 
}
