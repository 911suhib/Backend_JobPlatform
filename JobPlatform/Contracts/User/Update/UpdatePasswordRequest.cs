using System.ComponentModel.DataAnnotations;

namespace JobPlatformBackend.API.Contracts.User.Update
{
	public record UpdatePasswordRequest(
	[Required, MinLength(6)] string CurrentPassword,
	[Required, MinLength(6)] string NewPassword
);
}
