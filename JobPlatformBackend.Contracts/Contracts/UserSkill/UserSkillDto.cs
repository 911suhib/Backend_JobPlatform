using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Contracts.Contracts.UserSkill
{  	public record UserSkillDto(
		int SkillId,
		string SkillName
	);
}
