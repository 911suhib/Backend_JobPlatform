using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;


namespace JobPlatformBackend.Business.src.Services.Abstractions
{
	public interface ICloudinaryService
	{
		Task<DeletionResult?> DeleteImageAsync(string publicId);
		Task<ImageUploadResult> UploadImageAsync(IFormFile file, string folderName);
	}
}
