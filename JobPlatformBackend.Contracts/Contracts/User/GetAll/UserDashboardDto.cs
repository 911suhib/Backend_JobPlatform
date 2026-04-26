namespace JobPlatformBackend.API.Contracts.User.GetAll
{
	public class UserDashboardDto
	{
		public string FullName { get; set; }
		public string? Headline { get; set; }
		public string? ExpYears { get; set; }
		public List<string>? Skills { get; set; }

		// بيانات من جدول DashboardStats
		public int Commits { get; set; }
		public string Rank { get; set; }
		public string MarketValue { get; set; }

		public int Views { get; set; }
		// بيانات من جدول CareerArchitect
		public string? TargetTitle { get; set; }
		public int Progress { get; set; }
		public string? AIRecommendation { get; set; }
		public string? RoadmapData { get; set; }
	}
}
