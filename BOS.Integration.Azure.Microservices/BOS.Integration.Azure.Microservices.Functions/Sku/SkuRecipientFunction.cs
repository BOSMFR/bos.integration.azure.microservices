using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PrimeCargo;
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
        private readonly IBlobService blobService;
        private readonly IValidationService validationService;

        public SkuRecipientFunction(
            IServiceBusService serviceBusService,
            IValidationService validationService,
            IProductService productService, 
            IMapper mapper, 
            ILogService logService,
            IBlobService blobService)
        {
            this.serviceBusService = serviceBusService;
            this.productService = productService;
            this.logService = logService;
            this.mapper = mapper;
            this.blobService = blobService;
            this.validationService = validationService;
        }

        [FixedDelayRetry(3, "00:05:00")]
        [FunctionName("SkuRecipientFunction")]
        [return: ServiceBus("azure-topic-prime-cargo-wms-product-request", Connection = "serviceBus")]
        public async Task<Message> Run([ServiceBusTrigger("azure-topic-mesage-receiver-from-nav", "azure-sku-recipient-subscription", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("SkuRecipient function recieved the message from the topic");

                var erpMessageStatuses = new List<string>();
                var timeLines = new List<TimeLineDTO>();

                // Read file from blob storage
                string fileContent = await this.blobService.DownloadFileByFileNameAsync(mySbMsg);

                // Get product object from the topic message and create or update it in the storage
                var productDTO = JsonConvert.DeserializeObject<ProductDTO>(fileContent);

                string primeCargoIntegrationState = PrimeCargoProductHelper.GetPrimeCargoIntegrationState(productDTO.StartDatePrimeCargoExport);

                var createOrUpdateResponse = await this.productService.CreateOrUpdateProductAsync(productDTO, primeCargoIntegrationState);

                var product = createOrUpdateResponse.Item1;
                bool isNewObjectCreated = createOrUpdateResponse.Item2;

                //Create an erp message
                var erpInfo = this.mapper.Map<LogInfo>(product);

                erpMessageStatuses.Add(ErpMessageStatus.ReceivedFromErp);
                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.ErpMessageReceived, Status = TimeLineStatus.Information, DateTime = DateTime.UtcNow });

                if (primeCargoIntegrationState == PrimeCargoIntegrationState.Waiting)
                {
                    timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PreparingMessageCanceled + productDTO.StartDatePrimeCargoExport, Status = TimeLineStatus.Warning, DateTime = DateTime.UtcNow });

                    // Write erp messages and time lines to database
                    await this.logService.AddErpMessagesAsync(erpInfo, erpMessageStatuses);
                    await this.logService.AddTimeLinesAsync(erpInfo, timeLines);

                    log.LogError("Sku is not sent to PrimeCargo because startDatePrimeCargoExport was in wrong format or value was more than today");
                    return null;
                }

                // Check if the product received a response from prime cargo
                if (!isNewObjectCreated && !product.IsInvalid && product.PrimeCargoIntegration.State != PrimeCargoIntegrationState.DeliveredSuccessfully && product.PrimeCargoIntegration.State != PrimeCargoIntegrationState.Error)
                {
                    throw new Exception("Cannot proceed with updating SKU into prime cargo because it has not been delivered yet");
                }

                // Map the product to the prime cargo request object and check a description
                var primeCargoProduct = this.mapper.Map<PrimeCargoProductRequestDTO>(productDTO);

                primeCargoProduct.Description = PrimeCargoProductHelper.TrimPrimeCargoProductDescription(primeCargoProduct.Description);

                // Validate prime cargo product
                if (!validationService.Validate(primeCargoProduct))
                {
                    erpMessageStatuses.Add(ErpMessageStatus.Error);

                    // Set product as invalid
                    await productService.UpdateProductValidationStatusAsync(product.Id, true);

                    // Write erp messages and a line to database
                    await this.logService.AddErpMessagesAsync(erpInfo, erpMessageStatuses);
                    await this.logService.AddTimeLineAsync(erpInfo, TimeLineDescription.DataValidationFailed, TimeLineStatus.Error);

                    log.LogError("Prime Cargo object validation error occured");
                    return null;
                }

                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PrepareForServiceBus, Status = TimeLineStatus.Information, DateTime = DateTime.UtcNow });

                // Write erp messages and time lines to database
                await this.logService.AddErpMessagesAsync(erpInfo, erpMessageStatuses);
                await this.logService.AddTimeLinesAsync(erpInfo, timeLines);

                // Create a topic message
                var messageBody = new RequestMessage<PrimeCargoProductRequestDTO> { ErpInfo = erpInfo, RequestObject = primeCargoProduct };

                string primeCargoProductJson = JsonConvert.SerializeObject(messageBody);

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
