using System.Net;

namespace JobPlatformBackend.Domain.src.Exceptions
{
	public class ForbiddenException : AppException {

		public ForbiddenException(string message)
				: base(message, (int)HttpStatusCode.Forbidden)
		{
		}
	}

}
