using JobPlatform.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Infrastructure.src.Database
{
	 class AppDbContext:DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Company>Companies { get; set; }

		public DbSet<Application> Applications { get; set; }	

		public DbSet<Job> Jobs { get; set; }

		public DbSet<JobSkill> JobSkills { get; set; }

		public DbSet<UserSkill> UserSkills { get; set; }

		public DbSet<Skill> Skills { get; set; }

		public AppDbContext(DbContextOptions<AppDbContext> options):base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyConfigurationsFromAssembly(modelBuilder.GetType().Assembly);
		}

	}
}
