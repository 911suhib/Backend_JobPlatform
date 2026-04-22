using JobPlatformBackend.Contracts.Contracts.UserSkill;

namespace JobPlatformBackend.API.Contracts.User.Shared
{

	public record UserDto(
		int Id,
		string FName,
		string LName,
 		string Email,
		string Role,
		bool Active,
		string? PhoneNumber,
		string ?ProfileImageUrl,
		string ?Headline,
		string ?Location,
		string ?About,
		List<UserSkillDto> Skills
	);
}
