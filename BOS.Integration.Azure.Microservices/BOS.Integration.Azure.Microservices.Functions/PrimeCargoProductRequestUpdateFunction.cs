using BOS.Integration.Azure.Microservices.Domain.Enums;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions
{
    public class PrimeCargoProductRequestUpdateFunction
    {
        private readonly IPrimeCargoService primeCargoService;

        public PrimeCargoProductRequestUpdateFunction(IPrimeCargoService primeCargoService)
        {
            this.primeCargoService = primeCargoService;
        }


        [FunctionName("PrimeCargoProductRequestUpdateFunction")]
        [return: ServiceBus("azure-topic-prime-cargo-wms-product-response", Connection = "serviceBus")]
        public async Task<Message> Run([ServiceBusTrigger("azure-topic-prime-cargo-wms-product-request", "azure-sub-prime-cargo-product-update-request", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("PrimeCargoProductRequestUpdate function recieved the message from the topic");

                return await this.primeCargoService.CreateOrUpdatePrimeCargoProductAsync(mySbMsg, log, ActionType.Update);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
