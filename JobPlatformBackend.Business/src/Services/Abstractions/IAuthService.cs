using JobPlatformBackend.Contracts.Contracts.User.Create;
using JobPlatformBackend.Contracts.Contracts.User.GetAll;
using JobPlatformBackend.Domain.src.Entity;
 
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace JobPlatformBackend.Business.src.Services.Abstractions
{
	public interface IAuthService
	{
		Task SendVerificationCodeAsync(string email); 
 
		Task<AuthResultDto> AuthenticateUserAsync(UserCredentials userCredentials);

		Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
		Task<User> VerifyEmailAsync(string email, string code);

		Task<bool> ForgotPasswordAsync(string email);

		Task<bool> VerifyResetCodeAsync(string email, string code);

		Task<User> ResetPasswordAsync(string email, string code, string newPassword);
		Task<CreateUserResponse> CreateUserAsync(CreateUserRequests requests);
	}
}
