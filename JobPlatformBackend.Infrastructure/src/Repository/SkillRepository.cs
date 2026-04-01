using JobPlatformBackend.Domain.src.Abstractions;
using JobPlatformBackend.Domain.src.Entity;
using JobPlatformBackend.Infrastructure.src.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JobPlatformBackend.Infrastructure.src.Repository
{
	public class SkillRepository : BaseRepository<Skill>, ISkillRepository
	{
		private readonly AppDbContext _context;
		private readonly DbSet<Skill> _skills;
		private readonly ILogger<SkillRepository> _logger;

		public SkillRepository(AppDbContext context, ILogger<SkillRepository> logger)
			: base(context, logger)
		{
			_context = context;
			_logger = logger;
			_skills = _context.Set<Skill>();
		}

 		public async Task<Skill?> GetByIdAsync(int id)
		{
			return await _skills.FirstOrDefaultAsync(s => s.Id == id);
		}

 		public async Task<Skill?> GetByNameAsync(string name)
		{
			return await _skills
				.FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
		}

 		public async Task<IEnumerable<Skill>> GetAllAsync()
		{
			return await _skills.ToListAsync();
		}

 		public async Task AddAsync(Skill skill)
		{
			await _skills.AddAsync(skill);
		}

 		public void Delete(Skill skill)
		{
			_skills.Remove(skill);
		 
		}

		public async Task<List<Skill>> GetByNamesAsync(List<string> names)
		{
			var normalized = names.Select(n => n.ToLower()).ToList();

			return await _context.Skills
				.Where(s => normalized.Contains(s.Name.ToLower()))
				.ToListAsync();
		}
		public async Task AddRangeAsync(List<Skill> skills)
		{
			await _skills.AddRangeAsync(skills);
		}
	}
}