using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.Packshot
{
    public class PlytixPackshotUpdateCategoriesFunction
    {
        private readonly IPlytixService plytixService;
        private readonly IPackshotService packshotService;

        public PlytixPackshotUpdateCategoriesFunction(IPlytixService plytixService, IPackshotService packshotService)
        {
            this.plytixService = plytixService;
            this.packshotService = packshotService;
        }

        [FixedDelayRetry(3, "00:05:00")]
        [FunctionName("PlytixPackshotUpdateCategoriesFunction")]
        public async Task Run([ServiceBusTrigger("azure-topic-plytix-pim-packshot-response", "azure-sub-plytix-packshot-response-update-category", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("PlytixPackshotUpdateCategories function recieved the message from the topic");

                // Update asset categories in Plytix
                var messageObject = JsonConvert.DeserializeObject<ResponseMessage<PlytixPackshotUpdateCategoryDTO>>(mySbMsg);

                var updatePackshotCategoriesResponse = await this.plytixService.UpdatePackshotCategoriesAsync(messageObject, log);

                if (updatePackshotCategoriesResponse == null)
                {
                    return;
                }

                // Update cosmos db
                bool isSuccessfullyUpdated = await this.packshotService.UpdatePackshotFromPlytixInfoAsync(messageObject.ErpInfo.ObjectId, updatePackshotCategoriesResponse);

                if (isSuccessfullyUpdated)
                {
                    log.LogInformation("Packshot categories are successfully updated in Cosmos DB");
                }
                else
                {
                    log.LogError("Could not update the Packshot categories in Cosmos DB");
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
