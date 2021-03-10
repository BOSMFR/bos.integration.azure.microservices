using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Domain.Entities.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetByFilterAsync(ProductFilterDTO productFilter, string category = null);

        Task<List<Product>> GetAllByPrimeCargoIntegrationStateAsync(string state, string category = null);
    }
}
