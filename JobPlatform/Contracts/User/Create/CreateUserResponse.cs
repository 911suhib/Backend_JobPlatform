namespace JobPlatformBackend.API.Contracts.User.Create
{
	public record CreateUserResponse
	(
 	string Name,
 	string Email,
	string Role,
	bool Active,
	DateTime RegistrationDate)
	{

	}
}
