using Microsoft.AspNetCore.Http;

namespace JobPlatformBackend.Business.src.Services.Abstractions
{
	public interface IPdfService
	{
		string ExtractTextFromPdf(IFormFile file);
	}

}
