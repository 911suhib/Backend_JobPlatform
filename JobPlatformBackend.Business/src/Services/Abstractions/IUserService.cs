using JobPlatformBackend.API.Contracts.User.GetAll;
using JobPlatformBackend.API.Contracts.User.Shared;
using JobPlatformBackend.API.Contracts.User.Update;
using JobPlatformBackend.Domain.src.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Abstractions
{
	public interface IUserService
	{
		Task<IEnumerable<UserDto>>GetAllUserAsync(QueryOptions queryOptions);
		Task<UserResponse> GetUserByIdAsync(int userId);

		Task<UserResponse> GetUserByEmailAsync(string email);

		Task<UserResponse> UpdateUserAsync(int userId,UpdateUserRequest request);

		Task<bool> DeleteUserByIdAsync(int userId);
	}
}
