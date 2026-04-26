using JobPlatformBackend.Domain.src.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Infrastructure.src.Configuration
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.HasKey(x => x.Id);

			builder
		.HasOne(u => u.DashboardStats)
		.WithOne(s => s.User)
		.HasForeignKey<UserDashboardStats>(s => s.UserId);

			builder
				.HasOne(u => u.CareerPath)
				.WithOne(c => c.User)
				.HasForeignKey<CareerArchitect>(c => c.UserId);

			builder.Property(u => u.FName)
				  .IsRequired()
				  .HasMaxLength(100);

			builder.Property(u => u.LName)
				  .IsRequired()
				  .HasMaxLength(100);

			builder.Property(u => u.Id).ValueGeneratedOnAdd();

			// Email
			builder.Property(u => u.Email)
				.IsRequired()
				.HasMaxLength(255);

			builder.HasIndex(u => u.Email)
				.IsUnique();

			// Password
			builder.Property(u => u.HashPassword)
				.IsRequired()
				.HasMaxLength(500);

			// Phone
			builder.Property(u => u.PhoneNumber)
				.HasMaxLength(20);

			// Profile fields
			builder.Property(u => u.ProfileImageUrl)
				.HasMaxLength(500);

			builder.Property(u => u.CoverImageUrl)
				.HasMaxLength(500);

			builder.Property(u => u.Headline)
				.HasMaxLength(200);

			builder.Property(u => u.Location)
				.HasMaxLength(150);

			builder.Property(u => u.About)
				.HasMaxLength(2000);

			// Defaults
			builder.Property(u => u.Active)
				.HasDefaultValue(true);

			builder.Property(u => u.IsDeleted)
				.HasDefaultValue(false);

			// Enum
			builder.Property(u => u.Role)
				.IsRequired();

			 

			// Relationships
		 

			builder.HasMany(u => u.Applications)
				.WithOne(a => a.User)
				.HasForeignKey(a => a.UserId);

			builder.HasMany(u => u.UserSkills)
				.WithOne(us => us.User)
				.HasForeignKey(us => us.UserID);

	 
 

			builder.ToTable("Users");
		}
	}
}
