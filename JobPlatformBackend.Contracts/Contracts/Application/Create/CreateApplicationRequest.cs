namespace JobPlatformBackend.Contracts.Contracts.Application.Create
{
	public record CreateApplicationRequest(
		int JobId,
 		string? ResumeUrl  
	);
}