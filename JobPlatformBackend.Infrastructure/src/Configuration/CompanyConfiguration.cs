using JobPlatformBackend.Domain.src.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Infrastructure.src.Configuration
{
	public class CompanyConfiguration : IEntityTypeConfiguration<Company>
	{
		public void Configure(EntityTypeBuilder<Company> builder)
		{
			builder.ToTable("Companies");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.Name)
				.IsRequired()
				.HasMaxLength(150);

			builder.Property(x => x.Email)
				.IsRequired()
				.HasMaxLength(255);

			builder.Property(x => x.Descriptoin)
				.HasMaxLength(1000);

		 

			builder.HasMany(x => x.Jobs)
				.WithOne(j => j.Company)
				.HasForeignKey(j => j.CompanyId);
		}
	}
}
