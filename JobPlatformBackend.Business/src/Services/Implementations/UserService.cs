using JobPlatformBackend.API.Contracts.User.GetAll;
using JobPlatformBackend.API.Contracts.User.Shared;
using JobPlatformBackend.API.Contracts.User.Update;
using JobPlatformBackend.Business.src.Mappers;
using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.Shared;
using JobPlatformBackend.Contracts.Contracts.UserSkill;
using JobPlatformBackend.Domain.src.Abstractions;
using JobPlatformBackend.Domain.src.Entity;
using JobPlatformBackend.Infrastructure.src.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Implementations
{
	public class UserService(AppDbContext _context,IUserRepository _userRepository,BaseService<User,UserDto> _base,ILogger<UserService>_logger) : IUserService
	{
		public async Task<bool> AddSkillToUserAsync(int userId, string skillName)
		{	
			var user =await _context.Users.Include(u=>u.UserSkills).FirstOrDefaultAsync(u=>u.Id==userId);
			if (user == null) return false;
			var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Name.ToLower() == skillName.ToLower());
			if (skill == null)
			{
				skill= new Skill { Name = skillName };
				_context.Skills.Add(skill);
				await _context.SaveChangesAsync();
			}
			user.UserSkills.Add(new UserSkill { UserID = userId, SkillId = skill.Id });	
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteUserByIdAsync(int userId)
		{

			try
			{
				var user = await _userRepository.GetByIdAsync(userId);
			 

				var completed = await _userRepository.DeleteUser(user.Id);
				await _context.SaveChangesAsync();

				_logger.LogInformation("User with ID {UserId} deleted successfully.", userId);
				return completed;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting user with ID {UserId}", userId);
				throw;
			}
		}

		public async Task<IEnumerable<UserDto>> GetAllUserAsync(QueryOptions queryOptions )
		{
			try { 
		var users=	await _base.GetAll(queryOptions,DtoMapperUser.ToUserDto);
				_logger.LogInformation("Fetched {Count} users successfully.", users.Count());
				return users;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while fetching users with QueryOptions {@Options}", queryOptions);
				throw;
			}


		}

		public async Task<UserDto?> GetUserByEmailAsync(string email ,CancellationToken cancellationToken=default)
		{
			try
			{
				var userEntity = await _userRepository.Query()
		.Include(u => u.UserSkills)
			.ThenInclude(us => us.Skill)
		.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);


				if (userEntity == null) return null;
 
				var userDto = userEntity.ToUserIncludeDto();
 			 
				
				return userDto;
 			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while fetching user by Email");
				return null;
			}
		}

	 

		public async Task<UserDto?> GetUserByIdAsync(int id,CancellationToken cancellationToken)
		{
			try { 
			var user=await _userRepository.Query().Where(
				x=>x.Id==id &&x.IsDeleted==false
				).Select(DtoMapperUser.ToUserDto).FirstOrDefaultAsync(cancellationToken);
				_logger.LogInformation("Fetched User Successfuly");
			return user;
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "Error occurred while fetching user");
				throw;
			}
		}

	

		
		//[EmailAddress]
		//string? Email,
		//string? PhoneNumber,
		//string? ProfileImageUrl,
		//string? Headline,
		//string? Location,
		//string? About,
		//string? CoverImageUrl,
		//int? CompanyId
		public async Task<bool> UpdateUserAsync(int id, UpdateUserRequest updateUserRequest)
		{
			var user = await _userRepository.GetByIdAsync(id);
			if (user == null) return false;

			// جلب كل الخصائص من UpdateUserRequest
			var requestProps = typeof(UpdateUserRequest).GetProperties();
			var entityProps = typeof(User).GetProperties();

			foreach (var prop in requestProps)
			{
				var value = prop.GetValue(updateUserRequest);
				if (value != null) // إذا موجود في JSON
				{
					// البحث عن نفس الخاصية في User
					var userProp = entityProps.FirstOrDefault(p => p.Name == prop.Name);
					if (userProp != null && userProp.CanWrite)
					{
						userProp.SetValue(user, value);
					}
				}
			}

			await _userRepository.UpdateAsync(user);
			await _userRepository.SaveChangesAsync();

			return true;
		}






		public async Task<bool> UpdateUserProfilePictureAsync(int userId, string imageUrl)
		{
			try
			{
				var user= await _userRepository.GetByIdAsync(userId);
				if (user == null) return false;
				user.ProfileImageUrl = imageUrl;
				await _userRepository.UpdateAsync(user);
				await _userRepository.SaveChangesAsync();
				_logger.LogInformation("Profile picture for user {UserId} updated to {Url}", userId, imageUrl);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating profile picture for user {UserId}", userId);
				return false;
			}
		}
	}
}
