using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions
{
    public class SkuRecipientFunction
    {
        private readonly IProductService productService;
        private readonly IMapper mapper;

        public SkuRecipientFunction(IProductService productService, IMapper mapper)
        {
            this.productService = productService;
            this.mapper = mapper;
        }

        [FunctionName("SkuRecipientFunction")]
        [return: ServiceBus("azure-topic-prime-cargo-wms-product-request", Connection = "serviceBus")]
        public async Task<Message> Run([ServiceBusTrigger("azure-topic-mesage-receiver-from-nav", "azure-sku-recipient-subscription", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                // Get product objetc from the topic message and create or update it in the storage
                var productDTO = JsonConvert.DeserializeObject<ProductDTO>(mySbMsg);

                await this.productService.CreateOrUpdateProductAsync(productDTO);

                // Map the product to the prime cargo request object 
                var primeCargoProduct = this.mapper.Map<PrimeCargoProductRequestDTO>(productDTO); // ToDo - update mapping

                string primeCargoProductJson = JsonConvert.SerializeObject(primeCargoProduct);

                // Create a topic message
                byte[] messageBody = Encoding.UTF8.GetBytes(primeCargoProductJson);

                var topicMessage = new Message(messageBody);

                topicMessage.UserProperties.Add("type", "create"); // ToDo - check if prduct was created or updated

                return topicMessage;
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
