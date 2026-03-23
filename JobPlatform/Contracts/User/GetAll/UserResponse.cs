namespace JobPlatformBackend.API.Contracts.User.GetAll
{
	public record UserResponse
		(
		  int Id,
	string Name,
	string UserName,
	string Email,
	string Role,
	bool Active,
	string? PhoneNumber,
	string? ProfileImageUrl,
	string? Headline,
	string? Location,
	string? About,
	string? CoverImageUrl,
	int? CompanyId
		)
	;

}
