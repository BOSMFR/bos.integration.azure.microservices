using BOS.Integration.Azure.Microservices.Domain.Entities.Product;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product> GetByEanNoAsync(string eanNo);
    }
}
