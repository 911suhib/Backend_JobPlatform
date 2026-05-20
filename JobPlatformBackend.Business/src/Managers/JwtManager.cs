using JobPlatformBackend.Domain.src.Abstractions;
using JobPlatformBackend.Domain.src.Entity;
using JobPlatformBackend.Domain.src.Exceptions;
using JobPlatformBackend.Infrastructure.src.Database;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Managers
{
	public class JwtManager
	{
		private readonly JwtOptions _options;
		private readonly IUserRepository _userRepository;
		private readonly AppDbContext _dbcontext;
		private readonly IRefreshTokenRepository _refreshTokenRepository;

		public JwtManager(IOptions<JwtOptions> options, IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository, AppDbContext appDbContext)
		{
			_options = options.Value;
			_userRepository = userRepository;
			_dbcontext = appDbContext;
			_refreshTokenRepository = refreshTokenRepository;
		}


		public string GenerateAccessToken(User user)
		{
			var claims = new List<Claim> {
			new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
			new Claim (ClaimTypes.Email,user.Email),
			new Claim(ClaimTypes.Role,user.Role.ToString())
			};
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
			var singingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var securityTokenDescriptor = new SecurityTokenDescriptor
			{
				Issuer = _options.Issuer,
				Audience = _options.Audience,
				Expires = DateTime.UtcNow.AddMinutes(10),
				Subject = new ClaimsIdentity(claims),
				SigningCredentials = singingCredentials
			};
			var token = new JwtSecurityTokenHandler().CreateToken(securityTokenDescriptor);
			string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
			return tokenValue;
		}

		public string GenerateTempToken(string email)
		{
			var claims = new[] { new Claim(ClaimTypes.Email, email) };

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _options.Issuer,
				audience: _options.Audience,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(10),
				signingCredentials: creds
				);
			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public async Task<string> GenerateRefreshTokenAsync(User user)
		{
			var randomNumber = new byte[32];
			using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
			rng.GetBytes(randomNumber);

			string tokenString = Convert.ToBase64String(randomNumber);
			var refreshToken = new UserRefreshToken
			{
				UserId = user.Id,
				RefreshToken = tokenString,
				ExpiryDate = DateTime.UtcNow.AddDays(7),
				IsRevoked = false
			};


			await _refreshTokenRepository.AddRefreshToken(refreshToken);
			return tokenString;
		}

		
		public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken)
		{
 			var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

 			if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiryDate <= DateTime.UtcNow)
			{
				throw new BadRequestException("Invalid or expired refresh token.");
			}

 			var user = await _userRepository.GetByIdAsync(storedToken.UserId)
				?? throw new NotFoundException("User not found.");

 			storedToken.IsRevoked = true;
			await _refreshTokenRepository.DeleteRefreshTokenAsync(refreshToken);  

 			string newAccessToken = GenerateAccessToken(user);
			string newRefreshToken = await GenerateRefreshTokenAsync(user);

 			return new AuthResultDto
			{
				AccessToken = newAccessToken,
				RefreshToken = newRefreshToken
			};
		}
	}
}
