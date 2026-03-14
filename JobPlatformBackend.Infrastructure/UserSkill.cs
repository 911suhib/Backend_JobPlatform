namespace JobPlatform.Domain.Entity
{
	public class UserSkill:BaseEnitity
	{
		public int UserID { get; set; }
		public User User { get; set; }

		public int SkillId { get; set; }

		public Skill Skill { get; set; }

	}

}
