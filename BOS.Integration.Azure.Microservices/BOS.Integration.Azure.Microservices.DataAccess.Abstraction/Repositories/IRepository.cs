using BOS.Integration.Azure.Microservices.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories
{
    public interface IRepository<TEntity>
        where TEntity : BaseEntity
    {
        Task<ICollection<TEntity>> GetAllAsync(string partitionKey = null);
        
        Task<TEntity> GetByIdAsync(string id, string partitionKey = null);

        Task AddAsync(TEntity item, string partitionKey = null);

        Task AddRangeAsync(ICollection<TEntity> items, string partitionKey = null);

        Task UpdateAsync(TEntity item, string partitionKey = null);

        Task UpdateRangeAsync(ICollection<TEntity> items, string partitionKey = null);

        Task DeleteAsync(string id, string partitionKey = null);
    }
}
