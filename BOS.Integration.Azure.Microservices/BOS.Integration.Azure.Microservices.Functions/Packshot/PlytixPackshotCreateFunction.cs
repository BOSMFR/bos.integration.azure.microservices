using BOS.Integration.Azure.Microservices.Domain.Enums;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.Packshot
{
    public class PlytixPackshotCreateFunction
    {
        private readonly IPlytixService plytixService;

        public PlytixPackshotCreateFunction(IPlytixService plytixService)
        {
            this.plytixService = plytixService;
        }

        [FixedDelayRetry(3, "00:05:00")]
        [FunctionName("PlytixPackshotCreateFunction")]
        [return: ServiceBus("azure-topic-plytix-pim-packshot-response", Connection = "serviceBus")]
        public async Task<Message> Run([ServiceBusTrigger("azure-topic-plytix-pim-packshot-request", "azure-sub-plytix-packshot-create-request", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("PlytixPackshotCreate function recieved the message from the topic");

                return await this.plytixService.CreateOrUpdatePackshotAsync(mySbMsg, log, ActionType.Create);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
