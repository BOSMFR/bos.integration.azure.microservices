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
    public class SkuTimerFunction
    {
        private readonly IServiceBusService serviceBusService;
        private readonly IProductService productService;
        private readonly IMapper mapper;

        public SkuTimerFunction(IServiceBusService serviceBusService, IProductService productService, IMapper mapper)
        {
            this.serviceBusService = serviceBusService;
            this.productService = productService;
            this.mapper = mapper;
        }

        [FunctionName("SkuTimerFunction")]
        public async Task Run([TimerTrigger("0 0 1 * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"SkuTimer function executed at: {DateTime.Now}");

            var waitingProducts = await productService.GetAllByPrimeCargoIntegrationStateAsync(PrimeCargoIntegrationState.Waiting);

            var messages = new List<Message>();

            if (waitingProducts?.Count > 0)
            {
                foreach (var product in waitingProducts)
                {
                    // Create or update product into prime cargo if state is not equal to 'Waiting'                    
                    string primeCargoIntegrationState = PrimeCargoProductHelper.GetPrimeCargoIntegrationState(product.StartDatePrimeCargoExport);

                    if (primeCargoIntegrationState != PrimeCargoIntegrationState.Waiting)
                    {
                        // Map the product to the prime cargo request object and check a description
                        var primeCargoProduct = this.mapper.Map<PrimeCargoProductRequestDTO>(product);

                        primeCargoProduct.Description = PrimeCargoProductHelper.TrimPrimeCargoProductDescription(primeCargoProduct.Description);

                        // Create a topic message
                        string primeCargoProductJson = JsonConvert.SerializeObject(primeCargoProduct);

                        var messageProperties = new Dictionary<string, object> { { "type", product.PrimeCargoIntegration.Delivered ? "update" : "create" } };

                        messages.Add(this.serviceBusService.CreateMessage(primeCargoProductJson, messageProperties));
                    }
                }
            }

            if (messages.Count > 0)
            {
                await this.serviceBusService.SendMessagesToTopicAsync("azure-topic-prime-cargo-wms-product-request", messages);
            }
        }
    }
}
