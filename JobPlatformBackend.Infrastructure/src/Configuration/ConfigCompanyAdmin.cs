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
	internal class ConfigCompanyAdmin : IEntityTypeConfiguration<CompanyAdmin>
	{
		public void Configure(EntityTypeBuilder<CompanyAdmin> builder)
		{
			builder.HasKey(ca => new { ca.UserId, ca.CompanyId });
			builder.HasOne(ca => ca.User).WithMany(u => u.CompanyAdmins).HasForeignKey(ca => ca.UserId);
			builder.HasOne(ca => ca.Company).WithMany(c => c.CompanyAdmins).HasForeignKey(ca => ca.CompanyId);
		}
	}
}
