using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Domain.src.Entity
{


	public class User : SharedEnitity
	{
		public required string FName { get; set; }
		public required string LName { get; set; }
		public string? ProfileImagePublicId { get; set; }
		public required string Email { get; set; }
		public required string HashPassword { get; set; }
		public Role Role { get; set; }
		public bool Active { get; set; }
		public bool IsDeleted { get; set; }
		public string? PhoneNumber { get; set; }
		public string? ProfileImageUrl { get; set; }
		public string? Headline { get; set; }
		public string? Location { get; set; }
		public string? About { get; set; }
		public string? CoverImageUrl { get; set; }

		public ICollection<UserRefreshToken> RefreshTokens { get; set; } = new List<UserRefreshToken>();
		public string? EmailVerificationCode { get; set; }
		public DateTime? VerificationCodeExpiry { get; set; }

		public string? ExperienceYears { get; set; }
		public bool IsEmailVerified { get; set; } = false;

		// علاقات مهنية باقية
		public ICollection<Application> Applications { get; set; } = new List<Application>();
		public ICollection<CompanyAdmin> CompanyAdmins { get; set; } = new List<CompanyAdmin>();
		public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();

		// العلاقات الجديدة للداشبورد (Eager Loading - بدون virtual)
		public UserDashboardStats? DashboardStats { get; set; }  
		public CareerArchitect? CareerPath { get; set; }  

		// تم حذف الـ Posts والـ PostLikes والـ PostComments بناءً على طلبك
	}


}
