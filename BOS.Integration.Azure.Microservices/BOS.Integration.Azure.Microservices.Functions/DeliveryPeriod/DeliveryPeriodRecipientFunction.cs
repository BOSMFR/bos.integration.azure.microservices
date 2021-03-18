using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.DeliveryPeriod;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.DeliveryPeriod
{
    public class DeliveryPeriodRecipientFunction
    {
        private readonly ILogService logService;
        private readonly IDeliveryPeriodService deliveryPeriodService;
        private readonly IMapper mapper;
        private readonly IBlobService blobService;

        public DeliveryPeriodRecipientFunction(
            IDeliveryPeriodService deliveryPeriodService,
            IMapper mapper,
            ILogService logService,
            IBlobService blobService)
        {
            this.deliveryPeriodService = deliveryPeriodService;
            this.logService = logService;
            this.mapper = mapper;
            this.blobService = blobService;
        }

        [FixedDelayRetry(3, "00:05:00")]
        [FunctionName("DeliveryPeriodRecipientFunction")]
        public async Task Run([ServiceBusTrigger("azure-topic-mesage-receiver-from-nav", "azure-delivery-period-recipient-subscription", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("DeliveryPeriodRecipient function recieved the message from the topic");

                var timeLines = new List<TimeLineDTO>();
                var erpMessages = new List<string>();

                // Read file from blob storage
                string fileContent = await this.blobService.DownloadFileByFileNameAsync(mySbMsg);

                // Get delivery period object from the topic message and create or update it in the storage
                var deliveryPeriodDTO = JsonConvert.DeserializeObject<DeliveryPeriodDTO>(fileContent);

                var deliveryPeriod = await this.deliveryPeriodService.CreateOrUpdateDeliveryPeriodAsync(deliveryPeriodDTO);

                var erpInfo = this.mapper.Map<LogInfo>(deliveryPeriod);

                erpMessages.Add(ErpMessageStatus.ReceivedFromErp);
                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.ErpMessageReceived, Status = TimeLineStatus.Information, DateTime = DateTime.Now });

                // Write time lines to database
                await this.logService.AddErpMessagesAsync(erpInfo, erpMessages);
                await this.logService.AddTimeLinesAsync(erpInfo, timeLines);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
