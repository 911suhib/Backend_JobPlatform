using JobPlatformBackend.Contracts.Contracts.Company.Create;
using JobPlatformBackend.Domain.src.Abstractions;
using JobPlatformBackend.Domain.src.Entity;
using JobPlatformBackend.Infrastructure.src.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Infrastructure.src.Repository
{
	public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
	{
		private readonly ILogger _logger;
		private readonly AppDbContext _context;
		private readonly DbSet<Company> _company; 
		public CompanyRepository(AppDbContext context, ILogger<CompanyRepository> logger) : base(context, logger)
		{

			_context = context;
			_logger = logger;
			_company = _context.Set<Company>();
		}

		public async Task<bool> IsUserAdminOfCompanyAsync(int userId, int companyId)
		{
			return await _context.CompanyAdmins.AnyAsync(ca => ca.UserId == userId && ca.CompanyId == companyId);
		}
		
		public async Task AddAdminToCompanyAsync(CompanyAdmin companyAdmin)
		{
			await _context.CompanyAdmins.AddAsync(companyAdmin);
			await _context.SaveChangesAsync();
		}
		public async Task<IDbContextTransaction> BeginTransactionAsync()
		{
			return await _context.Database.BeginTransactionAsync();
		}
		public async Task<Company> CreateCompanyAsync(Company company)
		{
			await _context.Companies.AddAsync(company);
			await _context.SaveChangesAsync();
			return company;
		}
	}
}
