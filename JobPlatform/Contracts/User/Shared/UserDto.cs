namespace JobPlatformBackend.API.Contracts.User.Shared
{

	public record UserDto(
		int Id,
		string Name,
 		string Email,
		string Role,
		bool Active
	);
}
