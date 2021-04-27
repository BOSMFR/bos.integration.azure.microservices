using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot;
using BOS.Integration.Azure.Microservices.Domain.Enums;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.Packshot
{
    public class PlytixPackshotCreateFunction
    {
        private readonly IPlytixService plytixService;
        private readonly IPackshotService packshotService;
        private readonly IServiceBusService serviceBusService;

        public PlytixPackshotCreateFunction(
            IPlytixService plytixService, 
            IServiceBusService serviceBusService,
            IPackshotService packshotService)
        {
            this.plytixService = plytixService;
            this.serviceBusService = serviceBusService;
            this.packshotService = packshotService;
        }

        [FixedDelayRetry(3, "00:05:00")]
        [FunctionName("PlytixPackshotCreateFunction")]
        [return: ServiceBus("azure-topic-plytix-pim-packshot-response", Connection = "serviceBus")]
        public async Task<Message> Run([ServiceBusTrigger("azure-topic-plytix-pim-packshot-request", "azure-sub-plytix-packshot-create-request", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("PlytixPackshotCreate function recieved the message from the topic");

                // Create a Packshot in Plytix
                var messageObject = JsonConvert.DeserializeObject<RequestMessage<PlytixPackshotRequestDTO>>(mySbMsg);

                var packshotCreateResponse = await this.plytixService.CreateOrUpdatePackshotAsync(messageObject, log, ActionType.Create);

                if (packshotCreateResponse == null)
                {
                    return null;
                }

                // Update cosmos db
                bool isSuccessfullyUpdated = await this.packshotService.UpdatePackshotFromPlytixInfoAsync(messageObject.ErpInfo.ObjectId, packshotCreateResponse.PlytixResponseObject);

                if (isSuccessfullyUpdated)
                {
                    log.LogInformation("Packshot is successfully updated in Cosmos DB");
                }
                else
                {
                    log.LogError("Could not update the Packshot in Cosmos DB");
                }

                // Create a topic message
                var messageBody = new ResponseMessage<PlytixPackshotUpdateCategoryDTO> { ErpInfo = messageObject.ErpInfo, ResponseObject = packshotCreateResponse.PackshotUpdateCategoryDTO };

                string packshotResponseJson = JsonConvert.SerializeObject(messageBody);

                return this.serviceBusService.CreateMessage(packshotResponseJson);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
