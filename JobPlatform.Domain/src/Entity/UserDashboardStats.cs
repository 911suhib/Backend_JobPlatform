namespace JobPlatformBackend.Domain.src.Entity
{
	public class UserDashboardStats : SharedEnitity
		{
			public int UserId { get; set; }
			public int CodeCommits { get; set; }
			public string SkillRank { get; set; } = "N/A";
			public string MarketValue { get; set; } = "Calculating";
			public int ProfileViews { get; set; }

			// Navigation Property
			public User User { get; set; } = null!;
		}


}
