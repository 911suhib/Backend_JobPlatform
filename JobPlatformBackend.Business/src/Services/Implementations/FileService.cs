using JobPlatformBackend.Business.src.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Implementations
{
	public class FileService : IFileService
	{

		private readonly ICloudinaryService _cloudinaryService;

		public FileService(ICloudinaryService cloudinaryService)
		{
			_cloudinaryService = cloudinaryService;
		}

		public Task DeleteAsync(string publicId)
		{
			throw new NotImplementedException();
		}

		public async Task<(string Url, string PublicId)> ReplaceAsync(IFormFile file, string folder, string oldPublicId)
		{
			var result = await _cloudinaryService.UploadFileAsync(file, folder);

			if (!string.IsNullOrEmpty(oldPublicId))
			{
				await _cloudinaryService.DeleteFileAsync(oldPublicId);
			}

			return (result.SecureUrl.ToString(), result.PublicId);
		}

		public async Task<(string Url, string PublicId)> UploadAsync(IFormFile file, string folder)
		{
		var result = await _cloudinaryService.UploadFileAsync(file, folder);
			return (result.SecureUrl.ToString(), result.PublicId);
		}
	}
}
