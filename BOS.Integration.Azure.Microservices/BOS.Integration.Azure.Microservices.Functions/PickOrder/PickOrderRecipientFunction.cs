using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PickOrderEntity = BOS.Integration.Azure.Microservices.Domain.Entities.PickOrder.PickOrder;

namespace BOS.Integration.Azure.Microservices.Functions.PickOrder
{
    public class PickOrderRecipientFunction
    {
        private readonly IPickOrderService pickOrderService;
        private readonly IServiceBusService serviceBusService;
        private readonly IBlobService blobService;
        private readonly ILogService logService;
        private readonly IMapper mapper;

        public PickOrderRecipientFunction(
            IPickOrderService pickOrderService,
            IServiceBusService serviceBusService,
            IBlobService blobService,
            IMapper mapper,
            ILogService logService)
        {
            this.pickOrderService = pickOrderService;
            this.serviceBusService = serviceBusService;
            this.blobService = blobService;
            this.logService = logService;
            this.mapper = mapper;
        }

        [FixedDelayRetry(3, "00:05:00")]
        [FunctionName("PickOrderRecipientFunction")]
        [return: ServiceBus("azure-topic-prime-cargo-wms-pick-order-request", Connection = "serviceBus")]
        public async Task<Message> Run([ServiceBusTrigger("azure-topic-mesage-receiver-from-nav", "azure-pick-order-recipient-subscription", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("PickOrderRecipient function recieved the message from the topic");

                var timeLines = new List<TimeLineDTO>();

                // Read file from blob storage
                string fileContent = await this.blobService.DownloadFileByFileNameAsync(mySbMsg);

                // Get pick order object from the topic message and create it in the storage
                var pickOrderDTO = JsonConvert.DeserializeObject<PickOrderDTO>(fileContent);

                var createResponse = await this.pickOrderService.CreatePickOrderAsync(pickOrderDTO);

                var pickOrder = createResponse.Entity as PickOrderEntity;

                if (!createResponse.Succeeded && pickOrder == null)
                {
                    log.LogError(createResponse.Error);
                    return null;
                }

                // Create an erp message and time line
                LogInfo erpInfo = this.mapper.Map<LogInfo>(pickOrder);
                await this.logService.AddErpMessageAsync(erpInfo, ErpMessageStatus.ReceivedFromErp);

                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.ErpMessageReceived, Status = TimeLineStatus.Information, DateTime = DateTime.Now });

                if (!createResponse.Succeeded)
                {
                    timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.ErrorCreatingPickOrder + createResponse.Error, Status = TimeLineStatus.Error, DateTime = DateTime.Now });

                    // Write time lines to database
                    await this.logService.AddTimeLinesAsync(erpInfo, timeLines);

                    log.LogError(createResponse.Error);
                    return null;
                }

                // Validate pick order
                var messages = pickOrderService.ValidatePickOrder(pickOrder);

                if (messages.Count > 0)
                {
                    messages.ForEach(m => timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.ErrorValidationPickOrder + m, Status = TimeLineStatus.Error, DateTime = DateTime.Now }));

                    // Write time lines to database
                    await this.logService.AddTimeLinesAsync(erpInfo, timeLines);

                    log.LogError("Validation error");
                    return null;
                }

                // Map the pick order to the prime cargo request object
                var primeCargoPickOrder = this.mapper.Map<PrimeCargoPickOrderRequestDTO>(pickOrderDTO);

                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PrepareForServiceBus, Status = TimeLineStatus.Information, DateTime = DateTime.Now });

                // Write time lines to database
                await this.logService.AddTimeLinesAsync(erpInfo, timeLines);

                // Create a topic message
                var messageBody = new RequestMessage<PrimeCargoPickOrderRequestDTO> { ErpInfo = erpInfo, RequestObject = primeCargoPickOrder };

                string primeCargoProductJson = JsonConvert.SerializeObject(messageBody);

                var messageProperties = new Dictionary<string, object> { { "type", "create" } };

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
