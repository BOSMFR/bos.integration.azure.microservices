using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
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
        private readonly ILogService logService;
        private readonly IProductService productService;
        private readonly IMapper mapper;

        public SkuRecipientFunction(
            IServiceBusService serviceBusService, 
            IProductService productService, 
            IMapper mapper, 
            ILogService logService)
        {
            this.serviceBusService = serviceBusService;
            this.productService = productService;
            this.logService = logService;
            this.mapper = mapper;
        }

        [FunctionName("SkuRecipientFunction")]
        [return: ServiceBus("azure-topic-prime-cargo-wms-product-request", Connection = "serviceBus")]
        public async Task<Message> Run([ServiceBusTrigger("azure-topic-mesage-receiver-from-nav", "azure-sku-recipient-subscription", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("SkuRecipient function recieved the message from the topic");

                var timeLines = new List<TimeLineDTO>();

                // Get product objetc from the topic message and create or update it in the storage
                var productDTO = JsonConvert.DeserializeObject<ProductDTO>(mySbMsg);

                string primeCargoIntegrationState = PrimeCargoProductHelper.GetPrimeCargoIntegrationState(productDTO.StartDatePrimeCargoExport);

                var product = await this.productService.CreateOrUpdateProductAsync(productDTO, primeCargoIntegrationState);

                //Create an erp message
                var erpInfo = this.mapper.Map<LogInfo>(product);

                await this.logService.AddErpMessageAsync(erpInfo, ErpMessageStatus.ReceivedFromErp);
                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.ErpMessageReceived, Status = TimeLineStatus.Information, DateTime = DateTime.Now });

                if (primeCargoIntegrationState == PrimeCargoIntegrationState.Waiting)
                {
                    timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PreparingMessageCanceled + productDTO.StartDatePrimeCargoExport, Status = TimeLineStatus.Warning, DateTime = DateTime.Now });

                    // Write time lines to database
                    await this.logService.AddTimeLinesAsync(erpInfo, timeLines);

                    log.LogError("Sku is not sent to PrimeCargo because startDatePrimeCargoExport was in wrong format or value was more than today");
                    return null;
                }

                // Map the product to the prime cargo request object and check a description
                var primeCargoProduct = this.mapper.Map<PrimeCargoProductRequestDTO>(productDTO);

                primeCargoProduct.Description = PrimeCargoProductHelper.TrimPrimeCargoProductDescription(primeCargoProduct.Description);

                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PrepareForServiceBus, Status = TimeLineStatus.Information, DateTime = DateTime.Now });

                // Write time lines to database
                await this.logService.AddTimeLinesAsync(erpInfo, timeLines);

                // Create a topic message
                var messageBody = new PrimeCargoProductRequestMessage { ErpInfo = erpInfo, PrimeCargoProduct = primeCargoProduct };

                string primeCargoProductJson = JsonConvert.SerializeObject(messageBody);

                var messageProperties = new Dictionary<string, object> { { "type", product.PrimeCargoIntegration.Delivered ? "update" : "create" } };

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
