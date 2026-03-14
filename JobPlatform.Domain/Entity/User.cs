using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatform.Domain.Entity
{

	 
	public class User: SharedEnitity
	{
		public required string Name { get; set; }

		public required string Email { get; set; }

		public required string HashPassword { get; set; }

		public Role Role { get; set; }

		public bool Active { get; set; }

		public bool IsDeleted { get; set; }
		public string? PhoneNumber { get; set; }

		public int? CompanyId { get; set; }

		public Company? Company { get; set; }

		public ICollection<Application> Applications { get; set; } = new List<Application>();		
		public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();

	}

}
