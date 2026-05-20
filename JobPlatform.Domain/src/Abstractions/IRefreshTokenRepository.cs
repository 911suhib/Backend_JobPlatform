using JobPlatformBackend.Domain.src.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Domain.src.Abstractions
{
	public interface IRefreshTokenRepository
	{
		Task<string> AddRefreshToken(UserRefreshToken userRef);
		Task DeleteExpiredTokensAsync();
		Task DeleteRefreshTokenAsync(string refreshToken);
		Task DeleteTokensByUserIdAsync(int userId);
		Task<UserRefreshToken?> GetByTokenAsync(string refreshToken);
		Task RevokeAsync(int userId);
		Task<bool> ValidateRefreshTokenAsync(int userId, string refreshToken);
	}
}
