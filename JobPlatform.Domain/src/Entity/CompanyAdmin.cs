namespace JobPlatformBackend.Domain.src.Entity
{
	public class CompanyAdmin : SharedEnitity
	{
		public int UserId { get; set; }
		public User User { get; set; }

		public int CompanyId { get; set; }
		public Company Company { get; set; }

		// تقدر تضيف حقول إضافية هون مستقبلاً مثل RoleInCompany
		public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
	}

}
