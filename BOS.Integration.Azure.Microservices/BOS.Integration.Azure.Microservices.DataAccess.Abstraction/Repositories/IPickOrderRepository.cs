using BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder;
using BOS.Integration.Azure.Microservices.Domain.Entities.PickOrder;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories
{
    public interface IPickOrderRepository : IRepository<PickOrder>
    {
        Task<List<PickOrder>> GetByFilterAsync(PickOrderFilterDTO pickOrderFilter, string partitionKey = null);

        Task<List<PickOrder>> GetAllOpenAsync(string partitionKey = null);
    }
}
