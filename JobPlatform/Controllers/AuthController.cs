using JobPlatformBackend.Business.src.Managers;
using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Business.src.Services.Implementations;
using JobPlatformBackend.Contracts.Contracts.User.Create;
using JobPlatformBackend.Contracts.Contracts.User.GetAll;
using JobPlatformBackend.Contracts.Contracts.User.VerifyCode;
using JobPlatformBackend.Domain.src.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobPlatformBackend.API.Controllers
{

	[ApiController]
	[Route("api/v1/Auth")]
	public class AuthController(IAuthService _authService,IUserRepository _userRepository,JwtManager _jwtManager,IVerificationService _verification):ControllerBase
	{



		[HttpPost("login")]

		public async Task<IActionResult> Login(UserCredentials request)
		{
			var result = await _authService.AuthenticateUserAsync(request);
		 
			return Ok(new { Token = result });
		}
		

		[HttpPost("Regester")]
		public async Task<IActionResult> CreateAccount(CreateUserRequests requests)
		{


			if(!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var result = await _authService.CreateUserAsync(requests);
			return Ok(result);
		}

		[HttpPost("verify-email")]
		public async Task<IActionResult> VerifyEmail(VerifyEmailRequest request)
		{
			var user = await _userRepository.GetUserByEmailAsync(request.Email);
			if (user == null) { 
			throw new Exception("User not found.");
			}

			var result = await _verification.VerifyEmailCodeAsync(user,request.Code);
			if (!result)
			{
				return BadRequest(new { Message = "Invalid or expired Code." });
			}
			return Ok(new { Message = "Email verified successfully." });
		}

		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword([FromBody] string email)
		{
 			var result = await _verification.ForgetPasswordAsync(email);

			return Ok(new { message = "تم إرسال رمز التحقق إلى بريدك الإلكتروني 📧" });
		}
		[HttpPost("verify-reset-code")]
		public async Task<IActionResult> VerifyResetCode([FromBody] VerifyEmailRequest request)
		{
 			var isValid = await _verification.ValidateResetCodeAsync(request.Email, request.Code);

			if (isValid)
				return Ok(new { message = "الرمز صحيح، يمكنك الآن تغيير كلمة المرور ✅" });

			return BadRequest("الرمز غير صحيح أو انتهت صلاحيته");
		}

		[HttpPost("update-password")]
		public async Task<IActionResult> UpdatePassword(UpdatePasswordRequest request)
		{
			var check = await _verification.UpdatedPassword(request);
			if (!check)
				return BadRequest("حدث خطأ أثناء تحديث كلمة المرور");
			return Ok(new { message = "تم تحديث كلمة المرور بنجاح، يمكنك تسجيل الدخول الآن 🚀" });
		}


		[HttpPost("reset-password-final")]
		public async Task<IActionResult> ResetPasswordFinal([FromBody] ResetPasswordFinalRequest request)
		{
 			var result = await _verification.ExecutePasswordResetAsync(
				request.Email,
				request.Code,
				request.NewPassword
			);

			if (result)
				return Ok(new { message = "تم تحديث كلمة المرور بنجاح، يمكنك تسجيل الدخول الآن 🚀" });

			return BadRequest("حدث خطأ أثناء تحديث كلمة المرور");
		}
	}
}
