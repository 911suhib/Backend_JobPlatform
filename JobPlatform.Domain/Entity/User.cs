using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatform.Domain.Entity
{
	public enum TypeJob { FullTime, PartTime, Internship }
	public enum Role { Admin, User }
	public enum StatusApplication { Pending, Accepted, Rejected }
	public abstract class BaseEnitity
	{
		public int Id { get; set; }
	}

	public abstract class SharedEnitity:BaseEnitity
	{
 		public DateTime CreatedAt { get; set; }

	}

	 
	public class User: SharedEnitity
	{
		public string Name { get; set; }

		public string Email { get; set; }

		public string HashPassword { get; set; }

		public Role Role { get; set; }

		public string? PhoneNumber { get; set; }

		public int? CompanyId { get; set; }

		public Company Company { get; set; }

		public ICollection<Application> Applications { get; set; } = new List<Application>();		
		public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();




	}


	public class Company : SharedEnitity
	{
		public string Name { get; set; }
 		public string Descriptoin { get; set; }

		public ICollection<User> Admins { get; set; }=new List<User>();

		public ICollection<Job> Jobs { get; set; }= new List<Job>();
	}

	public class Job:SharedEnitity
	{
		public required string Title { get; set; }	

		public string? Description { get; set; }

		public decimal? Salary { get; set; }

		public string? Location { get; set; }

		public TypeJob TypeJop { get; set; }

		public string? ExperieceLevel { get; set; }

		public int CompanyId { get; set; }
		
		public Company Company { get; set; }

		public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
		public ICollection<Application> Applications { get; set; } = new List<Application>();


	}

	public class Application:SharedEnitity
	{
		public int JobId { get; set; }
		public Job Job { get; set; }

		public int UserId { get; set; }

		public User User { get; set; }

		public string CvUrl { get; set; }
		 
		public StatusApplication Status { get; set; }

	}
	public class Skill:BaseEnitity
	{
		public string Name { get; set; }

		public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();

		public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();

	}
	public class UserSkill:BaseEnitity
	{
		public int UserID { get; set; }
		public User User { get; set; }

		public int SkillId { get; set; }

		public Skill Skill { get; set; }

	}
	public class JobSkill
	{
		public int JobId { get; set; }
		public Job Job { get; set; }

		public int SkillId { get; set; }
		public Skill Skill { get; set; }
	}

}
