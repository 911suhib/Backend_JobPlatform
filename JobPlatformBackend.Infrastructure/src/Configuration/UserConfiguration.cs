using JobPlatform.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Infrastructure.src.Configuration
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.HasKey(x => x.Id);

			builder.Property(u => u.Name)
						  .IsRequired()
						  .HasMaxLength(100);

			builder.Property(u => u.Email)
			  .IsRequired()
			  .HasMaxLength(252);

			builder.Property(u => u.HashPassword)
				.IsRequired()
				.HasMaxLength(500);

			builder.Property(u => u.PhoneNumber)
				.HasMaxLength(20);
			builder.Property(u => u.Role)
				.IsRequired();

			builder.Property(u => u.Active)
				.HasDefaultValue(true);

			builder.Property(u => u.IsDeleted)
				.HasDefaultValue(false);
			builder.HasOne(u => u.Company)
			   .WithMany(c => c.Admins)
			   .HasForeignKey(u => u.CompanyId)
			   .OnDelete(DeleteBehavior.SetNull);


			builder.Property(x => x.Id).ValueGeneratedOnAdd();

			builder.HasIndex(u => u.Email)
			   .IsUnique();

			builder.ToTable("Users");
		}
	}
}
