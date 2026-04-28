namespace JobPlatformBackend.Domain.src.Entity
{
	public class Company : SharedEnitity
	{
		public string Name { get; set; }
 		public string Descriptoin { get; set; }
		public string? Email { get; set; }
 
		public string? Location { get; set; }
		public string? ProfileImagePublicId { get; set; }

		public string? LogoUrl { get; set; }
		public ICollection<Job> Jobs { get; set; }= new List<Job>();
		public ICollection<CompanyAdmin> CompanyAdmins { get; set; } = new List<CompanyAdmin>();

		public bool IsDeleted { get; set; } = false;


	}
	public class UserRoadmap
	{
		public int Id { get; set; }
		public int UserId { get; set; } // Foreign Key لجدول المستخدمين
		public string Headline { get; set; }
		public string About { get; set; }
		public string ExperienceYears { get; set; }
		public string MarketValue { get; set; }
		public string Recommendation { get; set; }
		public string TargetTitle { get; set; }
		public int Progress { get; set; } // نسبة الجاهزية (0-100)
		public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;

		// Navigation Property
		public ICollection<MissingSkill> MissingSkills { get; set; }
	}
	public class MissingSkill
	{
		public int Id { get; set; }
		public int UserRoadmapId { get; set; }
		public string SkillName { get; set; }
		public int ImpactPercentage { get; set; }

		// Navigation Property للكورسات
		public ICollection<RecommendedCourse> RecommendedCourses { get; set; }
	}
	public class RecommendedCourse
	{
		public int Id { get; set; }
		public int MissingSkillId { get; set; }
		public string Instructor { get; set; }
		public string CourseTitle { get; set; }
		public string Platform { get; set; } // YouTube, Udemy
		public string SearchQuery { get; set; } // عشان الفرونت إند يفتح الرابط فوراً
	}
}
