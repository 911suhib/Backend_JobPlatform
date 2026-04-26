using JobPlatformBackend.Business.src.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using UglyToad.PdfPig;

namespace JobPlatformBackend.Business.src.Services.Implementations
{
	public class PdfService : IPdfService // رح ننشئ الـ Interface بعد شوي
	{
		public string ExtractTextFromPdf(IFormFile file)
		{
			if (file == null || file.Length == 0) return string.Empty;

			using var stream = file.OpenReadStream();
			using var document = PdfDocument.Open(stream);

			// قراءة النص من كل الصفحات ودمجهم
			var text = string.Join(" ", document.GetPages().Select(p => p.Text));

			return text;
		}
	}

}
