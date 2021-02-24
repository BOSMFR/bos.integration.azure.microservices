using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using BOS.Integration.Azure.Microservices.Services.Helpers;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions
{
    public class SkuRecipientFunction
    {
        private readonly IServiceBusService serviceBusService;
        private readonly IProductService productService;
        private readonly IMapper mapper;

        public SkuRecipientFunction(IServiceBusService serviceBusService, IProductService productService, IMapper mapper)
        {
            this.serviceBusService = serviceBusService;
            this.productService = productService;
            this.mapper = mapper;
        }

        [FunctionName("SkuRecipientFunction")]
        [return: ServiceBus("azure-topic-prime-cargo-wms-product-request", Connection = "serviceBus")]
        public async Task<Message> Run([ServiceBusTrigger("azure-topic-mesage-receiver-from-nav", "azure-sku-recipient-subscription", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("SkuRecipient function recieved the message from the topic");

                // Get product objetc from the topic message and create or update it in the storage
                var productDTO = JsonConvert.DeserializeObject<ProductDTO>(mySbMsg);

                string primeCargoIntegrationState = PrimeCargoProductHelper.GetPrimeCargoIntegrationState(productDTO.StartDatePrimeCargoExport);

                bool isNewObjectCreated = await this.productService.CreateOrUpdateProductAsync(productDTO, primeCargoIntegrationState);

                if (primeCargoIntegrationState == PrimeCargoIntegrationState.Waiting)
                {
                    log.LogError("Sku is not sent to PrimeCargo because startDatePrimeCargoExport was in wrong format or value was more than today");
                    return null;
                }

                // Map the product to the prime cargo request object and check a description
                var primeCargoProduct = this.mapper.Map<PrimeCargoProductRequestDTO>(productDTO);

                primeCargoProduct.Description = PrimeCargoProductHelper.TrimPrimeCargoProductDescription(primeCargoProduct.Description);

                // Create a topic message
                string primeCargoProductJson = JsonConvert.SerializeObject(primeCargoProduct);

                var messageProperties = new Dictionary<string, object> { { "type", isNewObjectCreated ? "create" : "update" } };

                return this.serviceBusService.CreateMessage(primeCargoProductJson, messageProperties);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
