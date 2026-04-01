using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Domain.src.Entity
{


	public class User : SharedEnitity
	{
		public required string Name { get; set; }

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

		public ICollection<UserRefreshToken> RefreshTokens { get; set; }
		public string? EmailVerificationCode { get; set; }
		public DateTime? VerificationCodeExpiry { get; set; }
		public bool IsEmailVerified { get; set; } = false;
 
 		public ICollection<Application> Applications { get; set; } = new List<Application>();
		public ICollection<CompanyAdmin> CompanyAdmins { get; set; } = new List<CompanyAdmin>();
		public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();

		// منشورات المستخدم
		public ICollection<Post> Posts { get; set; } = new List<Post>();

		// اللايكات التي عملها المستخدم
		public ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

		// التعليقات التي كتبها المستخدم
		public ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();
	}


}
