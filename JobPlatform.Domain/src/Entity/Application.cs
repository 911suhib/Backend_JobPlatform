namespace JobPlatformBackend.Domain.src.Entity
{
	public class Application:SharedEnitity
	{
		public int JobId { get; set; }
		public Job Job { get; set; }

		public int UserId { get; set; }

		public User User { get; set; }

		public string CvPublicId { get; set; } = string.Empty;

		public string CvUrl { get; set; } = string.Empty;
		 
		public StatusApplication Status { get; set; }

	}

}
