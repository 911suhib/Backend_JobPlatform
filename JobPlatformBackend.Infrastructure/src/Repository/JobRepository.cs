using JobPlatformBackend.Contracts.Contracts.Jop;
using JobPlatformBackend.Domain.src.Abstractions;
using JobPlatformBackend.Domain.src.Entity;
using JobPlatformBackend.Infrastructure.src.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace JobPlatformBackend.Infrastructure.src.Repository
{
	public class JobRepository :  BaseRepository<Job>, IJobRepository
	{
		private readonly ILogger _logger;
		private readonly AppDbContext _context;
		private readonly DbSet<Job> _job;

		public JobRepository(AppDbContext context, ILogger<JobRepository> logger) : base(context, logger)
		{
			_context = context;
			_logger = logger;
			_job = _context.Set<Job>();
		}

		public async Task<(IEnumerable<JobResponseDto>Items,int TotalCount)> GetAllBySkillNameAsync(int page, int pageSize, string skill)
		{
			var query = _job.AsNoTracking().Where(j=>j.JobSkills.Any(js => EF.Functions.Like(js.Skill.Name, $"%{skill}%")));

			var totalCount = await query.CountAsync();

			var jobs= await query
				.OrderByDescending(j => j.CreatedAt)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.Select(x => new JobResponseDto(
					x.Id,
					x.Title,
					x.Description,
					x.Salary,
					x.Location,
					x.TypeJop.ToString(),
					x.ExperieceLevel.ToString(),
					x.CompanyId,
					x.Company.Name,
					x.JobSkills.Select(js => js.Skill.Name).ToList(),
					x.Applications.Count(),
					x.CreatedAt
				))
				.ToListAsync();
			return (jobs,totalCount);
		}

		public async Task<(IEnumerable<JobResponseDto>Items,int TotalCount)> GetByCompanyIdAsync(int companyId,int page,int pageSize)
		{
			var query = _job.AsNoTracking().Where(j => j.CompanyId == companyId);

			var totalCount = await query.CountAsync();


			var companies =await query
				.OrderByDescending(j=>j.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize)
				.Select(
				x => new JobResponseDto
				(
					x.Id,
					 x.Title,
					x.Description,
					x.Salary,
					x.Location,
					x.TypeJop.ToString(),
					x.ExperieceLevel.ToString(),
					x.CompanyId,
					x.Company.Name,
					x.JobSkills.Select(js => js.Skill.Name).ToList(),
				    x.Applications.Count(),
				    x.CreatedAt
				)
				).ToListAsync();


			return (companies,totalCount);
		}

		public async Task<IEnumerable<JobResponseDto>> GetJobsForUserAsync(int userId, int page,int pageSize)
		{
			var userSkills = await _context.Users.Where(u => u.Id == userId)
				.SelectMany(u => u.UserSkills.Select(us => us.Skill.Name)).ToListAsync();

			if (userSkills == null || !userSkills.Any())
			{
				return await _job.AsQueryable()
					.OrderByDescending(j => j.CreatedAt)
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.Select(x => new JobResponseDto(
						x.Id,
						x.Title,
						x.Description,
						x.Salary,
						x.Location,
						x.TypeJop.ToString(),
						x.ExperieceLevel.ToString(),
						x.CompanyId,
						x.Company.Name,
						x.JobSkills.Select(js => js.Skill.Name).ToList(),
						x.Applications.Count(),
						x.CreatedAt
					))
					.ToListAsync();
			}

			var jobs =await _job.AsQueryable().Where(j=>j.JobSkills.Any(js=>userSkills.Contains(js.Skill.Name)))
				.OrderByDescending(j=>j.JobSkills.Count(js=>userSkills.Contains(js.Skill.Name)))
				.ThenByDescending(j=>j.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).Select(x => new JobResponseDto(
			x.Id,
			x.Title,
			x.Description,
			x.Salary,
			x.Location,
			x.TypeJop.ToString(),
			x.ExperieceLevel.ToString(),
			x.CompanyId,
			x.Company.Name,
			x.JobSkills.Select(js => js.Skill.Name).ToList(),
			x.Applications.Count(),
			x.CreatedAt
		))
		.ToListAsync();

			return jobs;


		}
		public async Task<Job?> GetJobWithId(int id)
		{
			var job = await _job.Include(x => x.JobSkills).ThenInclude(js => js.Skill)
				.FirstOrDefaultAsync(j => j.Id == id);
			return job;
		}

		public async Task<Job?> GetWithDetailsAsync(int jobId)
		{
			return await _job.Include(j=>j.Company)
				.Include(j=>j.JobSkills)
				.ThenInclude(js=>js.Skill)
				.Include(j=>j.Applications)
				.FirstOrDefaultAsync(j => j.Id == jobId);
		}

		public async Task<IEnumerable<JobResponseDto>> SearchAsync(
	string? title = null,
	string? location = null,
	string? jobType = null,
	int? companyId = null,
	string? skill = null,
	int page = 1,
	int pageSize = 10)
		{
			var query = _job.AsQueryable();

			if (!string.IsNullOrEmpty(title))
				query = query.Where(x => EF.Functions.Like(x.Title, $"%{title}%"));

			if (!string.IsNullOrEmpty(location))
				query = query.Where(x => EF.Functions.Like(x.Location, $"%{location}%"));

			if (!string.IsNullOrEmpty(jobType))
				query = query.Where(x => x.TypeJop.ToString() == jobType);

			if (companyId.HasValue)
				query = query.Where(x => x.CompanyId == companyId);

			if (!string.IsNullOrEmpty(skill))
				query = query.Where(x => x.JobSkills.Any(js => js.Skill.Name.Contains(skill)));

			var jobs = await query
				.OrderBy(x => x.Id)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.Select(x => new JobResponseDto(
					x.Id,
					x.Title,
					x.Description,
					x.Salary,
					x.Location,
					x.TypeJop.ToString(),
					x.ExperieceLevel.ToString(),
					x.CompanyId,
					x.Company.Name,
					x.JobSkills.Select(js => js.Skill.Name).ToList(),
					x.Applications.Count(),
					x.CreatedAt
				))
				.ToListAsync();

			return jobs;
		}

		public async Task<List<Skill>> GetByNamesAsync(List<string> names)
		{
 			var lowerNames = names.Select(n => n.ToLower()).ToList();

			return await _context.Skills
				.Where(s => lowerNames.Contains(s.Name.ToLower()))
				.ToListAsync();
		}
	}
}
