using JobPlatformBackend.Contracts.Contracts.Application;
using JobPlatformBackend.Domain.src.Abstractions;
using JobPlatformBackend.Domain.src.Entity;
using JobPlatformBackend.Infrastructure.src.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Infrastructure.src.Repository
{
	public class ApplicationRepository : BaseRepository<Application>, IApplicationRepository
	{
		 private readonly ILogger _logger;
		private readonly AppDbContext _context;
		private readonly DbSet<Application> _application;
		public ApplicationRepository(AppDbContext context, ILogger<BaseRepository<Application>> logger) : base(context, logger)
		{
			_context = context;
			_logger = logger;
			_application = _context.Set<Application>();
		}
		public async Task<int> GetCountByJobIdAsync(int jobId)
		{
 			return await _context.Applications
				.CountAsync(a => a.JobId == jobId);
		}
		public async Task<IEnumerable<ApplicationResponse>> GetByJobIdAsync(int jobId,int pageNumber,int pageSize)
		{
			var applications = await _application.AsNoTracking().Where(a => a.JobId == jobId).OrderByDescending(a => a.CreatedAt)
				.Skip((pageNumber - 1) * pageSize).Take(pageSize)
				.Select(
				a=>new ApplicationResponse(a.Id, a.UserId, a.User.FName+" "+a.User.LName, a.User.Email, a.JobId, a.Job.Title, a.CreatedAt, a.CvUrl)
				).ToListAsync();
			return applications;
		}

		public Task<IEnumerable<Application>> GetByUserIdAsync()
		{
			throw new NotImplementedException();
		}

		public Task<Application> GetWithDetailsAsync(int applicationId)
		{
			throw new NotImplementedException();
		}

		public Task<Application> UpdateStatusAsync(int applicationId,StatusApplication status)
		{
			throw new NotImplementedException();
		}
	}
}
