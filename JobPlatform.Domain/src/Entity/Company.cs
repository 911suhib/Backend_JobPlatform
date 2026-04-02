namespace JobPlatformBackend.Domain.src.Entity
{
	public class Company : SharedEnitity
	{
		public string Name { get; set; }
 		public string Descriptoin { get; set; }
		public string Email { get; set; }
 
		public ICollection<Job> Jobs { get; set; }= new List<Job>();
		public ICollection<CompanyAdmin> CompanyAdmins { get; set; } = new List<CompanyAdmin>();

 
	}

}
