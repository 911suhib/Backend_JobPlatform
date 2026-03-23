using JobPlatformBackend.API.Contracts.User.Create;
using JobPlatformBackend.API.Contracts.User.Shared;
using JobPlatformBackend.Domain.src.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Mappers
{
	public static class DtoMapperUser
	{
		public static CreateUserResponse ToDto(this User user)
		{
			return new CreateUserResponse(user.Name, user.Email, user.Role.ToString(), user.Active,DateTime.UtcNow);
		}
		public static UserDto ToUserDto(this User user) {
			return new UserDto(user.Id, user.Name, user.Email, user.Role.ToString(), user.Active);
		}
	}
}
