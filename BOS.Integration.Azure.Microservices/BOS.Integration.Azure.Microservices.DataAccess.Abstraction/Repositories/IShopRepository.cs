using BOS.Integration.Azure.Microservices.Domain.Entities.Shopify;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories
{
    public interface IShopRepository : IRepository<Shop>
    {
        Task<List<Shop>> GetAllActiveAsync(string partitionKey = null);
    }
}
