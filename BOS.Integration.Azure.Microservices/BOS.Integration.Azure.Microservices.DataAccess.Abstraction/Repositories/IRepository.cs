using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        
        Task<T> GetByIdAsync(int id);

        Task AddAsync(T item);

        Task UpdateAsync(int id, T item);

        Task DeleteAsync(int id);
    }
}
