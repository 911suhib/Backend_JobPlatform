using JobPlatformBackend.Business.src.Services.Abstractions;
using JobPlatformBackend.Contracts.Contracts.Skill;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobPlatformBackend.API.Controllers
{
	[ApiController]
	[Route("api/v1/skill")]
	public class SkillContorller:ControllerBase
	{
		private readonly ISkillService _skillService;

		public SkillContorller(ISkillService skillService)
		{
			_skillService = skillService;
 		}
		[HttpGet("{skillName}/users")]
		public async Task<IActionResult> GetUsersBySkill(string skillName, CancellationToken cancellationToken)
		{
		var user= await _skillService.GetUsersBySkillNameAsync(skillName, cancellationToken);
			return Ok(user);
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> AddSkill(AddSkill request)
		{
			var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
			if (userClaim == null || !int.TryParse(userClaim.Value, out var userId))
				return Unauthorized();

			// استلم الكائن المكتمل
			var skillDto = await _skillService.AddSkillToUserAsync(userId, request.AddSkillRequest);

			if (skillDto == null)
				return BadRequest();

			// رجع الـ DTO للفرونت إند (هسا صار فيه ID و Name)
			return Ok(skillDto);
		}




		[HttpDelete("{skillId}")]
		[Authorize]
		public async Task<IActionResult> RemoveSkill(int skillId)
		{
			var user = User.FindFirst(ClaimTypes.NameIdentifier);

			if (user == null || !int.TryParse(user.Value, out var userId))
				return Unauthorized();

			var result = await _skillService.RemoveSkillFromUserAsync(userId, skillId);

			if (!result)
				return BadRequest();

			return Ok(new {Message="Success Delete Skill"});
		}
	}
}
