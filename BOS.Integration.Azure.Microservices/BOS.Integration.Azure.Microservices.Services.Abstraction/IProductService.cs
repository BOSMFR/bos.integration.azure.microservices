using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Domain.Entities.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IProductService
    {
        Task<List<Product>> GetProductByFilterAsync(ProductFilterDTO productFilter);

        Task<(Product, bool)> CreateOrUpdateProductAsync(ProductDTO productDTO, string primeCargoIntegrationState = null);

        Task<bool> UpdateProductFromPrimeCargoInfoAsync(PrimeCargoProductResponseDTO primeCargoResponse);

        Task UpdateProductValidationStatusAsync(string id, bool isInValid);

        Task<List<Product>> GetAllByPrimeCargoIntegrationStateAsync(string primeCargoIntegrationState);
    }
}
