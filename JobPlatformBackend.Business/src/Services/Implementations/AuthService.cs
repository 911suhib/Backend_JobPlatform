using JobPlatformBackend.API.Contracts.User.Shared;
using JobPlatformBackend.Business.src.Managers;
using JobPlatformBackend.Business.src.Mappers;
using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.Shared;
using JobPlatformBackend.Contracts.Contracts.User.Create;
using JobPlatformBackend.Contracts.Contracts.User.GetAll;
using JobPlatformBackend.Contracts.Contracts.User.Shared;
using JobPlatformBackend.Contracts.Contracts.UserSkill;
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
	public class AuthService : IAuthService
	{

		private readonly IUserRepository _userRepository;

		private readonly JwtManager _jwtManager;

		private readonly ISanitizerService _sanitizerService;

		private readonly IEmailService _emailService;

		private readonly IVerificationService _verificationService;
		private readonly IEmailTemplateService _emailTemplateService;

		private readonly IDashboardService _dashboardService;
		public AuthService(IDashboardService dashboardService,IEmailService emailService,IVerificationService verificationService,IEmailTemplateService emailTemplate,IUserRepository userRepository,JwtManager jwtManager,ISanitizerService sanitizerService)
		{
			_userRepository = userRepository;
			_jwtManager = jwtManager;
			_sanitizerService = sanitizerService;
			_emailService = emailService;
			_verificationService = verificationService;
			_emailTemplateService = emailTemplate;
			_dashboardService = dashboardService;
		}

		public async Task<string> AuthenticateUserAsync(UserCredentials userCredentials)
		{
			var user =await _userRepository.GetUserByEmailAsync(userCredentials.Email)??throw new BadRequestException("Invalid login credentials.");
			if (!user.IsEmailVerified)
			{
				throw new BadRequestException("The email is not verified");
			}
			if (user.IsDeleted)
			{
				throw new NotFoundException("The account is not found");
			}
			var isAuthenticated = PassswordService.VerifyPassword(user.HashPassword, userCredentials.Password);
			if (!isAuthenticated)
			{
				throw new BadRequestException("Invalid login credentials");
			}
			var subject = "مرحباً بك مجدداً في منصة دروب 🚀";

 			var html = _emailTemplateService.GetWelcomeEmail(user.FName+" "+user.LName);

			var sendeEmail = await _emailService.SendEmailAsync(user.Email, subject, html);
			string token = _jwtManager.GenerateAccessToken(user);
			return token;

		}

		public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequests requests)
		{
			
			var sanitizedDto=_sanitizerService.SanitizeDto(requests);
				var IsValidEmail = Validator.IsValidEmail(sanitizedDto.Email);
				if (!IsValidEmail)
				{
					throw new ArgumentException("Invalid Email address.");
				}

				var existingUser = await _userRepository.GetUserByEmailAsync(sanitizedDto.Email);
				if (existingUser is not null)
				{
					if (!existingUser.IsEmailVerified) {

					existingUser.FName = sanitizedDto.FName;
					existingUser.LName = sanitizedDto.LName;
					existingUser.PhoneNumber = sanitizedDto.PhoneNumber;
					existingUser.Location = sanitizedDto.Location;

					existingUser.HashPassword = PassswordService.HashPassword(sanitizedDto.Password);
					await _verificationService.SendEmailVerificationAsync(existingUser);
						
 					await _userRepository.SaveChangesAsync();
					throw new EmailNotVerifiedException("Verification code resent. Please verify your email.");
					}
					throw new BadRequestException("A user with this email alredy exist.");
				}

			try
			{
				var userEntity = new User
				{
					FName = sanitizedDto.FName,
					LName = sanitizedDto.LName,
					Email = sanitizedDto.Email,
					HashPassword = PassswordService.HashPassword(sanitizedDto.Password),
					PhoneNumber = sanitizedDto.PhoneNumber,
  					Location = sanitizedDto.Location,
    					Role = Role.User,
   					Active = true,
					IsDeleted = false,
					 
				};

				await _userRepository.AddAsync(userEntity);
				await _userRepository.SaveChangesAsync();
				await _dashboardService.InitializeDashboardAsync(userEntity.Id);
				await _verificationService.SendEmailVerificationAsync(userEntity);


				return userEntity.ToDto();
			}
			catch (Exception ex) {
				throw new Exception("Error in register process");
			}


		}

		public Task<bool> ForgotPasswordAsync(string email)
		{
			throw new NotImplementedException();
		}

		public Task<string> RefreshTokenAsync(string refreshToken)
		{
			throw new NotImplementedException();
		}

		public Task<User> ResetPasswordAsync(string email, string code, string newPassword)
		{
			throw new NotImplementedException();
		}

		public Task SendVerificationCodeAsync(string email)
		{
			throw new NotImplementedException();
		}

		public Task<User> VerifyEmailAsync(string email, string code)
		{
			throw new NotImplementedException();
		}

		public Task<bool> VerifyResetCodeAsync(string email, string code)
		{
			throw new NotImplementedException();
		}
 

	 
		 

	}
}
