using BOS.Integration.Azure.Microservices.Domain.Enums;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.PickOrder
{
    public class PrimeCargoPickOrderCreateFunction
    {
        private readonly IPrimeCargoService primeCargoService;

        public PrimeCargoPickOrderCreateFunction(IPrimeCargoService primeCargoService)
        {
            this.primeCargoService = primeCargoService;
        }

        [FixedDelayRetry(3, "00:05:00")]
        [FunctionName("PrimeCargoPickOrderCreateFunction")]
        [return: ServiceBus("azure-topic-prime-cargo-wms-pick-order-response", Connection = "serviceBus")]
        public async Task<Message> Run([ServiceBusTrigger("azure-topic-prime-cargo-wms-pick-order-request", "azure-sub-prime-cargo-pick-order-create-req", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("PrimeCargoPickOrderCreate function recieved the message from the topic");

                return null;
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
