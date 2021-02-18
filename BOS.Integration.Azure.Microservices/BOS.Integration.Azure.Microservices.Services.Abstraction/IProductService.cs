using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IProductService
    {
        Task<bool> CreateOrUpdateProductAsync(ProductDTO productDTO);
    }
}
