using JobPlatformBackend.Business.src.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobPlatformBackend.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DashboardController : ControllerBase
	{
		private readonly IDashboardService _dashboardService;

		// حقن السيرفس المسؤولة عن تجميع بيانات الداشبورد
		public DashboardController(IDashboardService dashboardService)
		{
			_dashboardService = dashboardService;
		}

		/// <summary>
		/// جلب كافة بيانات الداشبورد للمستخدم (البيانات الشخصية، الإحصائيات، وتوصيات الـ AI)
		/// </summary>
		[HttpGet("{userId}")]
		[Authorize]
		public async Task<IActionResult> GetUserDashboard()
		{
			try
			{
				var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);

				if (string.IsNullOrEmpty(nameIdentifier) || !int.TryParse(nameIdentifier, out int userId))
				{
					throw new UnauthorizedAccessException("Invalid or missing user token.");
				}

				var dashboardData = await _dashboardService.GetUserDashboardAsync(userId);

				return Ok(dashboardData);
			}
			catch (KeyNotFoundException ex)
			{
 				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
 				return StatusCode(500, new
				{
					message = "حدث خطأ أثناء جلب بيانات الداشبورد.",
					details = ex.Message
				});
			}
		}
	}
}