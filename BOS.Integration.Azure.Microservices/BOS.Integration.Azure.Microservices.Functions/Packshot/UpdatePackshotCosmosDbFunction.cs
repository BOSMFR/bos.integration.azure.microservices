using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot;
using BOS.Integration.Azure.Microservices.Domain.Entities.Packshot;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.Packshot
{
    public class UpdatePackshotCosmosDbFunction
    {
        private readonly IPackshotService packshotService;

        public UpdatePackshotCosmosDbFunction(IPackshotService packshotService)
        {
            this.packshotService = packshotService;
        }

        [FunctionName("UpdatePackshotCosmosDbFunction")]
        public async Task Run([ServiceBusTrigger("azure-topic-plytix-pim-packshot-response", "azure-sub-plytix-packshot-response-cosmos-db", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("UpdatePackshotCosmosDb function recieved the message from the topic");

                bool isSucceeded = false;

                var messageObject = JsonConvert.DeserializeObject<ResponseMessage<PlytixData<PlytixPackshotResponseData>>>(mySbMsg);

                isSucceeded = await packshotService.UpdatePackshotFromPlytixInfoAsync(messageObject.ErpInfo.ObjectId, messageObject.ResponseObject);

                if (isSucceeded)
                {
                    log.LogInformation("Packshot is successfully updated in Cosmos DB");
                }
                else
                {
                    log.LogError("Could not update the Packshot in Cosmos DB");
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
