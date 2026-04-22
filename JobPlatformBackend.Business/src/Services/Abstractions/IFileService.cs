using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Abstractions
{
	public interface IFileService
	{
		Task<(string Url, string PublicId)> UploadAsync(IFormFile file, string folder);

 		Task<(string Url, string PublicId)> ReplaceAsync(IFormFile file, string folder, string oldPublicId);

 		Task DeleteAsync(string publicId);
	}
}
