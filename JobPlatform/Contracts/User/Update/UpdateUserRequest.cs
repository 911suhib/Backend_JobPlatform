using System.ComponentModel.DataAnnotations;

namespace JobPlatformBackend.API.Contracts.User.Update
{
	public record UpdateUserRequest(
		string? Name,
		string? UserName,
		[EmailAddress] string? Email,
		string? PhoneNumber,
		string? ProfileImageUrl,
		string? Headline,
		string? Location,
		string? About,
		string? CoverImageUrl,
		int? CompanyId
	);
}
