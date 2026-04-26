using JobPlatformBackend.Business.src.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobPlatformBackend.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ResumeController : ControllerBase
	{
		private readonly IResumeService _resumeService;

		// حقن السيرفس اللي بتدير عملية التحليل كاملة
		public ResumeController(IResumeService resumeService)
		{
			_resumeService = resumeService;
		}

		/// <summary>
		/// استقبال ملف السيرة الذاتية وتحليلها وتحديث الداشبورد
		/// </summary>

		[HttpPost("upload-resume/{userId}")]
		[Authorize]
		
		public async Task<IActionResult> UploadResume(  IFormFile file)
		{
			var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (string.IsNullOrEmpty(nameIdentifier) || !int.TryParse(nameIdentifier, out int userId))
			{
				throw new UnauthorizedAccessException("Invalid or missing user token.");
			}
			// 1. التحقق الأولي من الملف
			if (file == null || file.Length == 0)
			{
				return BadRequest(new { message = "الرجاء اختيار ملف PDF صالح." });
			}

			// التأكد من نوع الملف
			var extension = Path.GetExtension(file.FileName).ToLower();
			if (extension != ".pdf")
			{
				return BadRequest(new { message = "النظام يدعم صيغة PDF فقط حالياً." });
			}

			try
			{
				// 2. إرسال الملف للسيرفس للقيام بـ (قراءة النص -> تحليل AI -> تحديث داتا بيز)
				var isProcessed = await _resumeService.ProcessResumeAsync(userId, file);

				if (isProcessed)
				{
					return Ok(new
					{
						message = "تم تحليل السيرة الذاتية بنجاح وتحديث بيانات الداشبورد! 🚀",
						timestamp = DateTime.UtcNow
					});
				}

				return BadRequest(new { message = "فشل في معالجة الملف، الرجاء المحاولة مرة أخرى." });
			}
			catch (Exception ex)
			{
				// 3. معالجة الأخطاء (مثل مشكلة في Gemini أو قاعدة البيانات)
				return StatusCode(500, new
				{
					message = "حدث خطأ أثناء معالجة السيرة الذاتية.",
					details = ex.Message
				});
			}
		}
	}
}
