using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.Shared;
using JobPlatformBackend.Contracts.Contracts.User.VerifyCode;
using JobPlatformBackend.Domain.src.Abstractions;
using JobPlatformBackend.Domain.src.Entity;
using JobPlatformBackend.Domain.src.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Implementations
{

	public class VerificationService (IUserRepository _userRepository,IEmailTemplateService _emailTemplateService,IEmailService _emailService) : IVerificationService
	{
		public string GenerateOtp()
		{
			byte[] randomNumber = new byte[6];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomNumber);

			int otpInt = Math.Abs(BitConverter.ToInt32(randomNumber, 0) % 1000000);
			return otpInt.ToString("D6");
		}

		public string HashOtp(string otp)
		{
			return BCrypt.Net.BCrypt.HashPassword(otp);
		}

		public bool VerifyOtp(string otp, string hashedOtp)
		{
			return BCrypt.Net.BCrypt.Verify(otp, hashedOtp);
		}

		public void SetVerification(User user, string hashedOtp, TimeSpan expiry)
		{
			user.EmailVerificationCode = hashedOtp;
			user.VerificationCodeExpiry = DateTime.UtcNow.Add(expiry);
			user.IsEmailVerified = false;
		}

		public async Task SendEmailVerificationAsync(User user)
		{
 			var otp = GenerateOtp();

			string hashedOtp = HashOtp(otp);
 			 SetVerification(user, hashedOtp, TimeSpan.FromMinutes(15));
			string subject = "💼 دروب | تأكيد هويتك المهنية";
			string htmlMessage = _emailTemplateService.GetVerificationEmail(otp);
			; var emailSent = await _emailService.SendEmailAsync(user.Email, subject, htmlMessage);
			if (!emailSent)
			{
				throw new ArgumentException("not found Email");
			}
 		}

		public async Task SendPasswordResetCodeAsync(User user)
		{
			var otp = GenerateOtp();

			string hashedOtp = HashOtp(otp);

			user.EmailVerificationCode = hashedOtp;
			user.VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(5);

			
			string subject = "🔒 رمز إعادة تعيين كلمة المرور";
			string htmlMessage = _emailTemplateService.GetResetPasswordEmail(otp);
			var emailSent = await _emailService.SendEmailAsync(user.Email, subject, htmlMessage);
			if (!emailSent) throw new ArgumentException("Failed to send reset password email.");
 		}

		public async Task<bool> VerifyEmailCodeAsync(User user, string otp)
		{
			if (user.VerificationCodeExpiry == null || user.VerificationCodeExpiry < DateTime.UtcNow)
				return false;

			if (string.IsNullOrEmpty(user.EmailVerificationCode))
				return false;
			var isValid = VerifyOtp(otp, user.EmailVerificationCode);

			if (isValid)
			{
				user.IsEmailVerified = true;
				user.EmailVerificationCode = null;
				user.VerificationCodeExpiry = null;
			}
			await	_userRepository.SaveChangesAsync();
			return true;
		}
		public async Task<bool> ConfirmResetPasswordAsync(string email, string otp, string newPassword)
		{
			var user = await _userRepository.GetUserByEmailAsync(email);

			if (user is null)
				throw new BadRequestException("User not found");

			if (user.VerificationCodeExpiry == null || user.VerificationCodeExpiry < DateTime.UtcNow)
				throw new BadRequestException("OTP expired");

			if (string.IsNullOrEmpty(user.EmailVerificationCode))
				throw new BadRequestException("No OTP found");

			var isValid = VerifyOtp(otp, user.EmailVerificationCode);

			if (!isValid)
				throw new BadRequestException("Invalid OTP");

			// Hash new password
			user.HashPassword = PassswordService.HashPassword(newPassword);

			// Clear OTP fields
			user.EmailVerificationCode = null;
			user.VerificationCodeExpiry = null;

			await _userRepository.SaveChangesAsync();

			return true;
		}

		public async Task<bool> ForgetPasswordAsync(string email)
		{
			var user = await _userRepository.GetUserByEmailAsync(email);

			if (user is null)
				throw new BadRequestException("User not found");

 			var otp = GenerateOtp();

 			string hashedOtp = HashOtp(otp);

 			user.EmailVerificationCode = hashedOtp;
			user.VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(5);

 			string subject = "🔒 رمز إعادة تعيين كلمة المرور";
			string htmlMessage = _emailTemplateService.GetResetPasswordEmail(otp);

			// Send email
			var emailSent = await _emailService.SendEmailAsync(user.Email, subject, htmlMessage);

			if (!emailSent)
				throw new ArgumentException("Failed to send reset password email.");

			// Save changes
			await _userRepository.SaveChangesAsync();

			return true;
		}

		public async Task<bool> ValidateResetCodeAsync(string email, string otp)
		{
			var user = await _userRepository.GetUserByEmailAsync(email);

			if (user is null || user.VerificationCodeExpiry < DateTime.UtcNow)
				throw new BadRequestException("الكود انتهت صلاحيته أو المستخدم غير موجود");

			// تحقق من الكود
			var isValid = VerifyOtp(otp, user.EmailVerificationCode);

			if (!isValid)
				throw new BadRequestException("Invalid Code");

		 
			return true;
		}
		public async Task<bool> ExecutePasswordResetAsync(string email, string otp, string newPassword)
		{
			var user = await _userRepository.GetUserByEmailAsync(email);

			if (user is null || user.VerificationCodeExpiry < DateTime.UtcNow)
				throw new BadRequestException("Expierd code");

 			var isValid = VerifyOtp(otp, user.EmailVerificationCode);
			if (!isValid)
				throw new BadRequestException("Error in process verification");

 			user.HashPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

 			user.EmailVerificationCode = null;
			user.VerificationCodeExpiry = null;

			await _userRepository.SaveChangesAsync();
			return true;
		}

		public async Task<bool> UpdatedPassword(UpdatePasswordRequest request)
		{
			var user = await _userRepository.GetUserByEmailAsync(request.Email);

			if (user == null)
				return false;

			var check = PassswordService.VerifyPassword( user.HashPassword, request.OldPassword);

			if (!check)
				return false;

			var newPassword = PassswordService.HashPassword(request.NewPassword);

			user.HashPassword = newPassword;
			await _userRepository.SaveChangesAsync();

			return true;

		}
		

		
	}
}
