using System.ComponentModel.DataAnnotations;

namespace JobPlatformBackend.API.Contracts.User.Create
{
	public record CreateUserRequest(
		[Required] string Name,
		[Required] string Email,
		[Required] string UserName,
		[Required,MinLength(4)] string Password,
		  string? PhoneNumber,
	string? ProfileImageUrl,
	string? Headline,
	string? Location,
	string? About,
	string? CoverImageUrl,
	int? CompanyId
		)
	{
	}
}
