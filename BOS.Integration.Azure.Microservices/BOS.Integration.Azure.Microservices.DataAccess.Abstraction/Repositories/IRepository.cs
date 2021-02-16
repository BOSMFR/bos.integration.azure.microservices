using BOS.Integration.Azure.Microservices.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories
{
    public interface IRepository<TEntity>
        where TEntity : BaseEntity
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        
        Task<TEntity> GetByIdAsync(string id);

        Task AddAsync(TEntity item);

        Task UpdateAsync(string id, TEntity item);

        Task DeleteAsync(string id);
    }
}
