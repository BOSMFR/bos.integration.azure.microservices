using BOS.Integration.Azure.Microservices.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories
{
    public interface IRepository<TEntity>
        where TEntity : BaseEntity
    {
        Task<ICollection<TEntity>> GetAllAsync(string partitionKey = null);

        Task<ICollection<V>> GetAllPropertyValuesAsync<V>(Expression<Func<TEntity, V>> selectExpression, string partitionKey = null);

        Task<ICollection<TEntity>> GetAllExceptAsync(ICollection<TEntity> items, string partitionKey = null);

        Task<TEntity> GetByIdAsync(string id, string partitionKey = null);

        Task AddAsync(TEntity item, string partitionKey = null);

        Task AddRangeAsync(ICollection<TEntity> items, string partitionKey = null);

        Task UpdateAsync(TEntity item, string partitionKey = null);

        Task UpdateRangeAsync(ICollection<TEntity> items, string partitionKey = null);

        Task DeleteAsync(string id, string partitionKey = null);

        Task DeleteRangeAsync(ICollection<string> ids, string partitionKey = null);
    }
}
