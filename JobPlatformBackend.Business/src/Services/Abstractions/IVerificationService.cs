using JobPlatformBackend.Contracts.Contracts.User.VerifyCode;
using JobPlatformBackend.Domain.src.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Abstractions
{
	public interface IVerificationService
	{
		string GenerateOtp();
		string HashOtp(string otp);
		bool VerifyOtp(string otp, string hashedOtp);
 

		void SetVerification(User user, string hashedOtp, TimeSpan expiry);
		Task SendEmailVerificationAsync(User user);
		Task SendPasswordResetCodeAsync(User user);
		 Task<bool> ExecutePasswordResetAsync(string email, string otp, string newPassword);
		 Task<bool> ForgetPasswordAsync(string email);

		 Task<bool> ValidateResetCodeAsync(string email, string otp);

		Task<bool> VerifyEmailCodeAsync(User user, string otp);
		Task<bool> UpdatedPassword(UpdatePasswordRequest request);
	}
}
