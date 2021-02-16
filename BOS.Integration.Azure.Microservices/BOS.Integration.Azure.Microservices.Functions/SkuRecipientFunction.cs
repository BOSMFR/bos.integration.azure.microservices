using AutoMapper;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Domain.Entities.Product;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions
{
    public class SkuRecipientFunction
    {
        private readonly IProductRepository repository;
        private readonly IMapper mapper;

        public SkuRecipientFunction(IProductRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [FunctionName("SkuRecipientFunction")]
        public async Task Run([ServiceBusTrigger("azure-topic-mesage-receiver-from-nav-dev", "azure-sku-recipient-subscription", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                var productDTO = JsonConvert.DeserializeObject<ProductDTO>(mySbMsg);

                var newProduct = this.mapper.Map<Product>(productDTO);

                newProduct.Category = NavObjectCategory.Sku;

                var product = await repository.GetByEanNoAsync(newProduct);

                if (product == null)
                {
                    await repository.AddAsync(newProduct);
                }
                else
                {
                    newProduct.Id = product.Id;
                    await repository.UpdateAsync(newProduct.Id, newProduct);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
