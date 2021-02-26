using BOS.Integration.Azure.Microservices.Domain.Entities.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetAllByPrimeCargoIntegrationStateAsync(string state);
    }
}
