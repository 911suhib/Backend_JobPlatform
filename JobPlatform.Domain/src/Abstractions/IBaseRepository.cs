using JobPlatformBackend.Domain.src.Common;
using JobPlatformBackend.Domain.src.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Domain.src.Abstractions
{
	public interface IBaseRepository<TEntity>  where TEntity : BaseEnitity
	{
		Task<IEnumerable<TResult>> GetAllAsync<TResult>(
		   QueryOptions queryOptions,
		   Expression<Func<TEntity, TResult>> selector);
				Task<TEntity> AddAsync(TEntity entity);
		Task<TEntity> UpdateAsync(object id,TEntity entity);

		Task<TEntity> DeleteAsync(TEntity entity);

		Task<TEntity> GetByIdAsync(object id);

	}
}
