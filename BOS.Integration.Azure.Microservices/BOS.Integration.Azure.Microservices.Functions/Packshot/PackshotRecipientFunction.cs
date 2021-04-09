using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PackshotEntity = BOS.Integration.Azure.Microservices.Domain.Entities.Packshot.Packshot;

namespace BOS.Integration.Azure.Microservices.Functions.Packshot
{
    public class PackshotRecipientFunction
    {
        private readonly IPackshotService packshotService;
        private readonly IServiceBusService serviceBusService;
        private readonly ILogService logService;
        private readonly IMapper mapper;

        public PackshotRecipientFunction(
            IPackshotService packshotService,
            IServiceBusService serviceBusService,
            IMapper mapper,
            ILogService logService)
        {
            this.packshotService = packshotService;

            this.serviceBusService = serviceBusService;
            this.logService = logService;
            this.mapper = mapper;
        }

        [FixedDelayRetry(3, "00:05:00")]
        [FunctionName("PackshotRecipientFunction")]
        [return: ServiceBus("azure-topic-plytix-pim-packshot-request", Connection = "serviceBus")]
        public async Task<Message> Run([ServiceBusTrigger("azure-topic-mesage-receiver-from-nav", "azure-packshot-recipient-subscription", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("PackshotRecipient function recieved the message from the topic");

                var timeLines = new List<TimeLineDTO>();

                // Get packshot object from the topic message and create it in the storage
                var packshotDTO = JsonConvert.DeserializeObject<PackshotDTO>(mySbMsg);

                var createResponse = await this.packshotService.CreatePackshotAsync(packshotDTO);

                var packshot = createResponse.Entity as PackshotEntity;

                if (!createResponse.Succeeded && packshot == null)
                {
                    log.LogError(createResponse.Error);
                    return null;
                }

                // Create an erp message and time line
                LogInfo erpInfo = this.mapper.Map<LogInfo>(packshot);
                await this.logService.AddErpMessageAsync(erpInfo, ErpMessageStatus.ReceivedFromSsis);

                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.SsisMessageReceived, Status = TimeLineStatus.Information, DateTime = DateTime.Now });

                if (!createResponse.Succeeded)
                {
                    timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.ErrorCreatingPackshot + createResponse.Error, Status = TimeLineStatus.Error, DateTime = DateTime.Now });

                    // Write time lines to database
                    await this.logService.AddTimeLinesAsync(erpInfo, timeLines);

                    log.LogError(createResponse.Error);
                    return null;
                }

                // Check image format and product brand
                if (packshot.Imageformat?.Id != PackshotDefault.ImageFormat || string.IsNullOrEmpty(packshot.Product?.Brand))
                {
                    if (packshot.Imageformat?.Id != PackshotDefault.ImageFormat)
                    {
                        timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PackshotWrongImageFormat, Status = TimeLineStatus.Error, DateTime = DateTime.Now });
                    }

                    if (string.IsNullOrEmpty(packshot.Product?.Brand))
                    {
                        timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PackshotBrandMissed, Status = TimeLineStatus.Error, DateTime = DateTime.Now });
                    }

                    // Write time lines to database
                    await this.logService.AddTimeLinesAsync(erpInfo, timeLines);

                    log.LogError(TimeLineDescription.PackshotWrongImageFormat);
                    return null;
                }

                // Map the packshot to the plytix request object
                var plytixPackshot = this.mapper.Map<PlytixPackshotRequestDTO>(packshot);

                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PrepareForServiceBus, Status = TimeLineStatus.Information, DateTime = DateTime.Now });

                // Write time lines to database
                await this.logService.AddTimeLinesAsync(erpInfo, timeLines);

                // Create a topic message
                var messageBody = new RequestMessage<PlytixPackshotRequestDTO> { ErpInfo = erpInfo, RequestObject = plytixPackshot };

                string requestMessageJson = JsonConvert.SerializeObject(messageBody);

                var messageProperties = new Dictionary<string, object> { { "type", "create" } };

                return this.serviceBusService.CreateMessage(requestMessageJson, messageProperties);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
