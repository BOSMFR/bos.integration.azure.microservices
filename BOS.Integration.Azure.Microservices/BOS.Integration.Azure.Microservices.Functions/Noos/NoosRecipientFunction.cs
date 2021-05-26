using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Noos;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.Noos
{
    public class NoosRecipientFunction
    {
        private readonly ILogService logService;
        private readonly INoosService noosService;
        private readonly IMapper mapper;
        private readonly IBlobService blobService;

        public NoosRecipientFunction(
            INoosService noosService,
            IMapper mapper,
            ILogService logService,
            IBlobService blobService)
        {
            this.noosService = noosService;
            this.logService = logService;
            this.mapper = mapper;
            this.blobService = blobService;
        }

        [FixedDelayRetry(3, "00:05:00")]
        [FunctionName("NoosRecipientFunction")]
        public async Task Run([ServiceBusTrigger("azure-topic-mesage-receiver-from-nav", "azure-noos-recipient-subscription", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("NoosRecipient function recieved the message from the topic");

                var timeLines = new List<TimeLineDTO>();
                var erpMessages = new List<string>();

                // Read file from blob storage
                string fileContent = await this.blobService.DownloadFileByFileNameAsync(mySbMsg);

                // Get noos object from the topic message and create or update it in the storage
                var noosDTO = JsonConvert.DeserializeObject<NoosDTO>(fileContent);

                var noos = await this.noosService.CreateOrUpdateNoosAsync(noosDTO);

                var erpInfo = this.mapper.Map<LogInfo>(noos);

                erpMessages.Add(ErpMessageStatus.ReceivedFromErp);
                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.ErpMessageReceived, Status = TimeLineStatus.Information, DateTime = DateTime.UtcNow });

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
