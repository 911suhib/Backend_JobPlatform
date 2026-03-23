using JobPlatformBackend.Domain.src.Abstractions;
using JobPlatformBackend.Domain.src.Common;
using JobPlatformBackend.Domain.src.Entity;
using JobPlatformBackend.Infrastructure.src.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Infrastructure.src.Repository
{
	public class UserRepository : BaseRepository<User>, IUserRepository
	{
		private readonly AppDbContext ApplicationDbContext;
		private readonly DbSet<User> _Users;
		private readonly ILogger<BaseRepository<User>> _logger;

		public UserRepository(AppDbContext applicatoinDbContext,ILogger<BaseRepository<User>>logger):base(applicatoinDbContext,logger)
		{
		ApplicationDbContext = applicatoinDbContext;
			_logger = logger;
			_Users = ApplicationDbContext.Set<User>();
		}
		 
		public async Task<User> CreateAdminAsync(User user)
		{
			user.Role = Role.Admin;
			await _Users.AddAsync(user);
			return user;
		}

		public async Task<bool> DeleteUser(int id)
		{
			var user = await _Users.FindAsync(id);
			if (user== null) {return false;}
			user.IsDeleted= true;
			return true;
		}

		public async Task<User?> GetUserByEmailAsync(string email)
		{
			return await _Users
				   .AsNoTracking()
				   .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
		}

		public async Task<User> UpdatePassword(string email, string PasswordHash)
		{
			var user = await _Users.FirstOrDefaultAsync(u => u.Email == email);
			if (user == null)
				return null;

			user.HashPassword = PasswordHash;
			return user;
		}
	}
}
