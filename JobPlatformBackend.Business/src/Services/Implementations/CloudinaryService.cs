using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using JobPlatformBackend.Business.src.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Implementations
{
	public class CloudinaryService : ICloudinaryService
	{
		private readonly Cloudinary _cloudinary;
		public CloudinaryService(Cloudinary cloudinary)
		{
			_cloudinary = cloudinary;
		}
		public async Task<ImageUploadResult> UploadImageAsync(IFormFile file, string folderName)
		{
			var uploadResult=new ImageUploadResult();
			if(file?.Length>0)
			{
				using var stream = file.OpenReadStream();
				var uploadParams = new ImageUploadParams
				{
					File = new FileDescription(file.FileName, stream),
					Folder = folderName,
					Transformation = new Transformation().Quality("auto").FetchFormat("auto")
				};
					
				uploadResult =await _cloudinary.UploadAsync(uploadParams);
			}
			return uploadResult;
		}

		public async Task<DeletionResult?> DeleteImageAsync(string publicId)
		{
			if (string.IsNullOrEmpty(publicId)) return null;

 			var deleteParams = new DeletionParams(publicId);

 			var result = await _cloudinary.DestroyAsync(deleteParams);

			return result;
		}

		public async Task<DeletionResult?> DeleteFileAsync(string publicId)
		{
			if (string.IsNullOrEmpty(publicId)) return null;

			var deleteParams = new DeletionParams(publicId)
			{
				ResourceType = ResourceType.Raw
			};
			var result = await _cloudinary.DestroyAsync(deleteParams);

			return result;
		}

		public async Task<RawUploadResult> UploadFileAsync(IFormFile file, string folderName)
		{
			var uploadResult = new RawUploadResult();
			if (file?.Length > 0)
			{
				using var stream = file.OpenReadStream();
				var uploadParams = new RawUploadParams
				{
					File = new FileDescription(file.FileName, stream),
					Folder = folderName,
					AccessMode = "public"
				};
				uploadResult = await _cloudinary.UploadAsync(uploadParams);
			}
			return uploadResult;
		}
	}
}
