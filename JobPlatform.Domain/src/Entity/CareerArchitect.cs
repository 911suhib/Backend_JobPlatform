namespace JobPlatformBackend.Domain.src.Entity
{
	public class CareerArchitect : SharedEnitity
		{
			public int UserId { get; set; }
			public string? TargetTitle { get; set; }
		public int ProgressPct { get; set; } = 0;
			public string? AIRecommendation { get; set; }

			public string? RoadmapJson{ get; set; }

			// Navigation Property
			public User User { get; set; } = null!;
		
	}


}
