 using JobPlatformBackend.API.Contracts.User.Shared;
using JobPlatformBackend.API.Contracts.User.Update;
using JobPlatformBackend.Contracts.Contracts.User.Create;
using JobPlatformBackend.Contracts.Contracts.UserSkill;
using JobPlatformBackend.Domain.src.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
namespace JobPlatformBackend.Business.src.Mappers
{
	public static class DtoMapperUser
	{
		public static CreateUserResponse ToDto(this User user)
		{
			return new CreateUserResponse(user.FName, user.LName, user.Email, user.Role.ToString(), user.Active, DateTime.UtcNow);
		}

		public static UserDto ToUserIncludeDto(this User user)
		{
			// استخدمنا ?. لنتأكد إنه القائمة مش نل، واستخدمنا الـ ! لنتأكد من وجود الـ Skill
			var skills = user.UserSkills?
				.Select(s => new UserSkillDto(s.Skill!.Id, s.Skill!.Name))
				.ToList() ?? new List<UserSkillDto>();

			return new UserDto(
				user.Id,
				user.FName,
				user.LName,
				user.Email,
				user.Role.ToString(),
				user.Active,
				user.PhoneNumber,
				user.ProfileImageUrl,
				user.Headline,
				user.Location,
				user.About,
				skills);
		}

		public static Expression<Func<User, UserDto>> ToUserDto =
			user => new UserDto(
				user.Id,
				user.FName,
				user.LName,
				user.Email,
				user.Role.ToString(),
				user.Active,
				user.PhoneNumber,
				user.ProfileImageUrl,
				user.Headline,
				user.Location,
				user.About,
				// في الـ Expression بنستخدم علامة التعجب ! لنخبر المترجم أن البيانات موجودة
				user.UserSkills!.Select(s => new UserSkillDto(s.Skill!.Id, s.Skill!.Name)).ToList() ?? new List<UserSkillDto>());
	}
}