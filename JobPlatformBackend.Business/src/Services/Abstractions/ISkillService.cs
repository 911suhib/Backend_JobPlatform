using JobPlatformBackend.API.Contracts.User.Shared;
using JobPlatformBackend.Contracts.Contracts.Skill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Business.src.Services.Abstractions
{
	public interface ISkillService
	{
		Task<IEnumerable<UserDto>> GetUsersBySkillNameAsync(string skillName, CancellationToken cancellationToken = default);

		Task<SkillDto> AddSkillToUserAsync(int userId, string skillName); Task<bool> RemoveSkillFromUserAsync(int userId, int skillId);

		Task<IEnumerable<SkillDto>> GetAllSkillsAsync(CancellationToken cancellationToken = default);
	}
}
