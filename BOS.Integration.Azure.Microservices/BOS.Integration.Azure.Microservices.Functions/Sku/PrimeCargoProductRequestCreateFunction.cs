using BOS.Integration.Azure.Microservices.Domain.Enums;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions
{
    public class PrimeCargoProductRequestCreateFunction
    {
        private readonly IPrimeCargoService primeCargoService;

        public PrimeCargoProductRequestCreateFunction(IPrimeCargoService primeCargoService)
        {
            this.primeCargoService = primeCargoService;
        }

        [FixedDelayRetry(3, "00:05:00")]
        [FunctionName("PrimeCargoProductRequestCreateFunction")]
        [return: ServiceBus("azure-topic-prime-cargo-wms-product-response", Connection = "serviceBus")]
        public async Task<Message> Run([ServiceBusTrigger("azure-topic-prime-cargo-wms-product-request", "azure-sub-prime-cargo-product-create-request", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("PrimeCargoProductRequestCreate function recieved the message from the topic");

                return await this.primeCargoService.CreateOrUpdatePrimeCargoProductAsync(mySbMsg, log, ActionType.Create);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
