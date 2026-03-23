using JobPlatformBackend.API.Contracts.User.GetAll;
using JobPlatformBackend.API.Contracts.User.Shared;
using JobPlatformBackend.API.Contracts.User.Update;
using JobPlatformBackend.Business.src.Mappers;
using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Domain.src.Abstractions;
using JobPlatformBackend.Domain.src.Common;
using JobPlatformBackend.Infrastructure.src.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Implementations
{
	public class UserService(AppDbContext _context,IUserRepository _userRepository) : IUserService
	{
		public async Task<bool> DeleteUserByIdAsync(int userId)
		{
			var user= await _userRepository.GetByIdAsync(userId);
			if (user == null) {
				return false;

			}
			var completed = await _userRepository.DeleteUser(user.Id);
			await _context.SaveChangesAsync();
			return completed;
		}

		public async Task<IEnumerable<UserDto>> GetAllUserAsync(QueryOptions queryOptions)
		{
			var users=await _userRepository.GetAllAsync(queryOptions,
				x=> x.ToUserDto()
				);
			return users;
			 
		}

		public Task<UserResponse> GetUserByEmailAsync(string email)
		{
			throw new NotImplementedException();
		}

		public Task<UserResponse> GetUserByIdAsync(int userId)
		{
			throw new NotImplementedException();
		}

		public Task<UserResponse> UpdateUserAsync(int userId, UpdateUserRequest request)
		{
			throw new NotImplementedException();
		}
	}
}
