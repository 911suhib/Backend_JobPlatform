using System.Net;

namespace JobPlatformBackend.Domain.src.Exceptions
{
	public class BadRequestException : AppException
	{
		public BadRequestException(string message) : base(message, (int)HttpStatusCode.BadRequest) { }
	}

}
