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
	public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEnitity
	{

		private readonly AppDbContext _applicatoinDbContext;

		private readonly DbSet<TEntity> _dbset;

		private readonly ILogger <BaseRepository<TEntity>> _logger;

		public BaseRepository(AppDbContext applicationDbContext,ILogger<BaseRepository<TEntity>> logger)
		{
		_applicatoinDbContext = applicationDbContext;
		_dbset = _applicatoinDbContext.Set<TEntity>();
			_logger = logger;
		}

		public async Task<TEntity> AddAsync(TEntity entity)
		{
			try {
			var entry=await _dbset.AddAsync(entity);
			await _applicatoinDbContext.SaveChangesAsync();
				return entry.Entity;
			} 
			catch (DbUpdateException ex) {
				_logger.LogError(ex, "Error adding {Entity}", typeof(TEntity).Name);

				throw;
			}
		}

		public Task<TEntity> DeleteAsync(TEntity entity)
		{
			try {
				_dbset.Remove(entity);
				return Task.FromResult(entity);
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "Error deleting {Entity}", typeof(TEntity).Name); throw;
			}
		}
		public async Task<IEnumerable<TResult>> GetAllAsync<TResult>(
	QueryOptions queryOptions,
	Expression<Func<TEntity, TResult>> selector)
		{
			if (queryOptions == null)
				throw new ArgumentNullException(nameof(queryOptions));

			IQueryable<TEntity> query = _dbset.AsNoTracking();

			// ===== Search =====
			if (!string.IsNullOrEmpty(queryOptions.SearchKeyword))
			{
				var searchableProperties = typeof(TEntity)
					.GetProperties()
					.Where(p => p.PropertyType == typeof(string))
					.ToList();

				if (searchableProperties.Any())
				{
					var parameter = Expression.Parameter(typeof(TEntity), "entity");
					var orConditions = new List<Expression>();

					foreach (var property in searchableProperties)
					{
						var propertyAccess = Expression.Property(parameter, property);
						var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
						var callToLower = Expression.Call(propertyAccess, toLowerMethod);

						var patternExpr = Expression.Constant($"%{queryOptions.SearchKeyword.ToLower()}%");
						var likeMethod = typeof(DbFunctionsExtensions).GetMethod(
							"Like",
							new[] { typeof(DbFunctions), typeof(string), typeof(string) }
						)!;
						var efFunctions = Expression.Property(null, typeof(EF), nameof(EF.Functions));

						var likeCall = Expression.Call(likeMethod, efFunctions, callToLower, patternExpr);
						orConditions.Add(likeCall);
					}

					var combined = orConditions.Aggregate(Expression.OrElse);
					var lambda = Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
					query = query.Where(lambda);
				}
			}

			// ===== Sorting =====
			var propertyInfo = typeof(TEntity).GetProperty(queryOptions.SortBy);
			if (propertyInfo != null)
			{
				query = ApplySorting(query, queryOptions.SortBy, queryOptions.SortDescending);
			}

			// ===== Pagination =====
			int pageNumber = Math.Max(1, queryOptions.PageNumber);
			int pageSize = Math.Min(100, queryOptions.PageSize);
			query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

			// ===== Projection =====
			return await query.Select(selector).ToListAsync();
		}
		public static IQueryable<TEntity> ApplySorting<TEntity>(IQueryable<TEntity> query,string sortBy,bool descending)
		{
			var property = typeof(TEntity).GetProperty(sortBy)
	?? throw new ArgumentException($"Property '{sortBy}' not found on type '{typeof(TEntity).Name}'");



			var parameter =Expression.Parameter(typeof(TEntity),"x");
			var propertyAccess = Expression.Property(parameter,property);

			var lambda=Expression.Lambda(propertyAccess, parameter);
			var methodName=descending?"OrderByDescending":"OrderBy";

			var result = Expression.Call
				(
				typeof(Queryable), methodName, new Type[] { typeof(TEntity), property.PropertyType }, query.Expression, Expression.Quote(lambda)
				);
			return query.Provider.CreateQuery<TEntity>(result);
		}

		public async Task<TEntity?> GetByIdAsync(object id)
		{
			return await _dbset.FindAsync(id)??throw new ArgumentNullException("not found");
		}

		public async Task<TEntity> UpdateAsync(object id,TEntity entity)
		{
			var existingEntity=await GetByIdAsync(id);
			if (existingEntity == null)
				return null;
			_applicatoinDbContext.Entry(existingEntity).CurrentValues.SetValues(entity);

			return existingEntity;
		}

		 
	}
}
