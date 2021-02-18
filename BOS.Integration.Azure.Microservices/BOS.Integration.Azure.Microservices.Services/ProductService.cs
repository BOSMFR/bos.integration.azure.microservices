using AutoMapper;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Domain.Entities.Product;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository repository;
        private readonly IMapper mapper;

        public ProductService(IProductRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<bool> CreateOrUpdateProductAsync(ProductDTO productDTO, string primeCargoIntegrationState = null)
        {
            bool isNewObjectCreated = false;

            var newProduct = this.mapper.Map<Product>(productDTO);

            newProduct.Category = NavObjectCategory.Sku;

            var product = await repository.GetByEanNoAsync(newProduct.EanNo, newProduct.Category);

            if (product == null)
            {
                newProduct.PrimeCargoIntegration = new PrimeCargoIntegration
                {
                    Delivered = false,
                    State = primeCargoIntegrationState ?? PrimeCargoIntegrationState.NotDelivered
                };

                await repository.AddAsync(newProduct);

                isNewObjectCreated = true;
            }
            else
            {
                newProduct.Id = product.Id;
                await repository.UpdateAsync(newProduct.Id, newProduct);
            }

            return isNewObjectCreated;
        }

        public async Task UpdateProductFromPrimeCargoInfoAsync(PrimeCargoProductResponseDTO primeCargoResponse)
        {
            var product = await repository.GetByEanNoAsync(primeCargoResponse.EnaNo, NavObjectCategory.Sku);

            if (product != null)
            {
                product.PrimeCargoProductId = primeCargoResponse.ProductId;

                product.PrimeCargoIntegration = new PrimeCargoIntegration
                {
                    Delivered = primeCargoResponse.Success,
                    State = primeCargoResponse.Success ? PrimeCargoIntegrationState.DeliveredSuccessfully : PrimeCargoIntegrationState.Error
                };

                await repository.UpdateAsync(product.Id, product);
            }
        }
    }
}
