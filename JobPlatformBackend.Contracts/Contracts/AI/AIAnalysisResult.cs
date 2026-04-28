namespace JobPlatformBackend.Contracts.Contracts.AI
{
	public class AIAnalysisResult
	{
		public bool isResume { get; set; }
		public string headline { get; set; }
		public string about { get; set; }
		public string experienceYears { get; set; }
		public string marketValue { get; set; }
		public string recommendation { get; set; }
		public string targetTitle { get; set; }
		public List<string> skills { get; set; }
		public int progress { get; set; }
		public List<SkillGap> missingSkillsWithImpact { get; set; }
		public List<RoadmapItem> roadmapData { get; set; } // هاد بضل زي ما هو
	}

	public class SkillGap
	{
		public string skillName { get; set; }
		public int impactPercentage { get; set; }
	}

	public class RoadmapItem
	{
		public string skillName { get; set; }
		// ⬇️ التعديل هنا: لأن البرومت برجع 4 كورسات لكل مهارة ⬇️
		public List<CourseResource> topResources { get; set; }
	}

	public class CourseResource
	{
		public string instructor { get; set; }
		public string courseTitle { get; set; }
		public string platform { get; set; }
		public string url { get; set; }
	}
}