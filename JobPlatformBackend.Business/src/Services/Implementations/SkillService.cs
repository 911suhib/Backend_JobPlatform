using JobPlatformBackend.API.Contracts.User.Shared;
using JobPlatformBackend.Business.src.Mappers;
using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.Skill;
using JobPlatformBackend.Domain.src.Abstractions;
using JobPlatformBackend.Domain.src.Entity;
using JobPlatformBackend.Domain.src.Exceptions;
using JobPlatformBackend.Infrastructure.src.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Implementations
{
	public class SkillService(AppDbContext _context,ISkillRepository _skillRepository, IUserRepository _userRepository, ILogger<UserService> _logger) : ISkillService
	{
		public async Task<SkillDto> AddSkillToUserAsync(int userId, string skillName)
		{
			skillName = skillName.Trim();

			var user = await _context.Users
				.Include(u => u.UserSkills)
				.FirstOrDefaultAsync(u => u.Id == userId);

			if (user == null)
				throw new NotFoundException("User not found"); // أفضل من false عشان تعرف العلة

			var skill = await _skillRepository.GetByNameAsync(skillName);

			if (skill == null)
			{
				skill = new Skill { Name = skillName };
				await _skillRepository.AddAsync(skill);
				await _context.SaveChangesAsync(); // هون بتولد الـ ID للمهارة الجديدة
			}

			if (user.UserSkills.Any(us => us.SkillId == skill.Id))
				throw new DuplicateSkillException("This skill is already held by user");

			user.UserSkills.Add(new UserSkill
			{
				UserID = userId,
				SkillId = skill.Id
			});

			await _context.SaveChangesAsync();

			// التعديل الجوهري: نرجع الـ DTO كامل مع الـ ID
			return new SkillDto(skill.Id, skill.Name);
		}

		public async Task<IEnumerable<SkillDto>> GetAllSkillsAsync(CancellationToken cancellationToken = default)
		{
			var skills = await _skillRepository.GetAllAsync();

			return skills.Select(s => new SkillDto(s.Id, s.Name));
		}

		public async Task<IEnumerable<UserDto>> GetUsersBySkillNameAsync(string skillName, CancellationToken cancellationToken = default)
		{
			//var users = await _context.Users.Where(u => u.UserSkills.Any(us => us.Skill.Name.ToLower().Contains(skillName.ToLower())))
			//	.Select(u => u.ToUserIncludeDto()).ToListAsync(cancellationToken);

			var users = await _userRepository.Query()
	.Where(u => u.UserSkills.Any(us =>
		EF.Functions.Like(us.Skill.Name, $"%{skillName}%")))
	.Select(u => u.ToUserIncludeDto())
	.ToListAsync(cancellationToken);

			return users;

		}

		public async Task<bool> RemoveSkillFromUserAsync(int userId, int skillId)
		{
			var user = await _context.Users.Include(u => u.UserSkills).FirstOrDefaultAsync(u => u.Id == userId);

			if (user == null)
				return false;

			var userSkill = user.UserSkills
				.FirstOrDefault(us => us.SkillId == skillId);

			if (userSkill == null)
				return false;

			user.UserSkills.Remove(userSkill);

			await _context.SaveChangesAsync();
			return true;

		}

	}
}
