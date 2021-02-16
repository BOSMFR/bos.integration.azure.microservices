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

        public async Task CreateOrUpdateProductAsync(ProductDTO productDTO)
        {
            var newProduct = this.mapper.Map<Product>(productDTO);

            newProduct.Category = NavObjectCategory.Sku;

            var product = await repository.GetByEanNoAsync(newProduct);

            if (product == null)
            {
                newProduct.PrimeCargoIntegration = new PrimeCargoIntegration
                {
                    Delivered = false,
                    State = PrimeCargoIntegrationState.NotDelivered
                };

                await repository.AddAsync(newProduct);
            }
            else
            {
                newProduct.Id = product.Id;
                await repository.UpdateAsync(newProduct.Id, newProduct);
            }
        }
    }
}
