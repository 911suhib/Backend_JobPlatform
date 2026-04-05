using JobPlatformBackend.API.Contracts.User.GetAll;
using JobPlatformBackend.API.Contracts.User.Shared;
using JobPlatformBackend.API.Contracts.User.Update;
using JobPlatformBackend.Contracts.Contracts.Shared;
 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Abstractions
{
	public interface IUserService
	{

		Task<bool> UpdateUserAsync(int id, UpdateUserRequest updateUserRequest);
		Task<IEnumerable<UserDto>>GetAllUserAsync(QueryOptions queryOptions);

		Task<UserDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken=default);
		Task<bool> DeleteUserByIdAsync(int userId);

		Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
		Task<bool> UpdateUserProfilePictureAsync(int userId, string imageUrl);
	}
}
