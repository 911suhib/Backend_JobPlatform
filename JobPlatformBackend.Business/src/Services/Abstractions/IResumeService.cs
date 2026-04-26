using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Abstractions
{
	public interface IResumeService
	{
		public  Task<bool> ProcessResumeAsync(int userId, IFormFile file);
	}
}
