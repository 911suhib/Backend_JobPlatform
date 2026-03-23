namespace JobPlatformBackend.API.Contracts.User.GetAll
{
	public record GetUsersRequest(bool IsActiveOnly = true, int PageNumber=1,int Limit = 50);

}
