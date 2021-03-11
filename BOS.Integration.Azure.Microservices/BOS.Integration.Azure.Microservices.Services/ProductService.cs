using AutoMapper;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Domain.Entities.Product;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System;
using System.Collections.Generic;
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

        public async Task<List<Product>> GetProductByFilterAsync(ProductFilterDTO productFilter)
        {
            productFilter.FromDate ??= DateTime.MinValue;
            productFilter.ToDate ??= DateTime.MaxValue;

            return await this.repository.GetByFilterAsync(productFilter);
        }

        public async Task<(Product, bool)> CreateOrUpdateProductAsync(ProductDTO productDTO, string primeCargoIntegrationState = null)
        {
            bool isNewObjectCreated = false;

            var newProduct = this.mapper.Map<Product>(productDTO);

            var product = await repository.GetByIdAsync(newProduct.EanNo, NavObjectCategory.Sku);

            newProduct.PrimeCargoIntegration = GetPrimeCargoIntegration(product?.PrimeCargoIntegration?.Delivered, primeCargoIntegrationState);

            if (product == null)
            {
                newProduct.Category = NavObjectCategory.Sku;
                newProduct.ReceivedFromErp = DateTime.Now;

                await repository.AddAsync(newProduct, newProduct.Category);

                isNewObjectCreated = true;
            }
            else
            {
                newProduct.Id = product.Id;
                newProduct.Category = product.Category;
                newProduct.PrimeCargoProductId = product.PrimeCargoProductId;
                newProduct.ReceivedFromErp = product.ReceivedFromErp;

                await repository.UpdateAsync(newProduct.Id, newProduct, newProduct.Category);
            }

            return (newProduct, isNewObjectCreated);
        }

        public async Task<List<Product>> GetAllByPrimeCargoIntegrationStateAsync(string primeCargoIntegrationState)
        {
            return await repository.GetAllByPrimeCargoIntegrationStateAsync(primeCargoIntegrationState, NavObjectCategory.Sku);
        }

        public async Task<bool> UpdateProductFromPrimeCargoInfoAsync(PrimeCargoProductResponseDTO primeCargoResponse)
        {
            var product = await repository.GetByIdAsync(primeCargoResponse.EnaNo, NavObjectCategory.Sku);

            if (product == null)
            {
                return false;
            }

            if (primeCargoResponse.ProductId.HasValue)
            {
                product.PrimeCargoProductId = primeCargoResponse.ProductId;
            }

            product.PrimeCargoIntegration.Delivered = product.PrimeCargoIntegration.Delivered || primeCargoResponse.Success;
            product.PrimeCargoIntegration.State = primeCargoResponse.Success ? PrimeCargoIntegrationState.DeliveredSuccessfully : PrimeCargoIntegrationState.Error;

            await repository.UpdateAsync(product.Id, product, product.Category);

            return true;
        }

        private PrimeCargoIntegration GetPrimeCargoIntegration(bool? delivered, string primeCargoIntegrationState = null) =>
            new PrimeCargoIntegration
            {
                Delivered = delivered ?? false,
                State = primeCargoIntegrationState ?? PrimeCargoIntegrationState.NotDelivered
            };
    }
}
