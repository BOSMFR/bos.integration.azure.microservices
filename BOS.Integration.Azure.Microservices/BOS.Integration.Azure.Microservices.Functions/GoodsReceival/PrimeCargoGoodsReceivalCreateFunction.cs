using BOS.Integration.Azure.Microservices.Domain.Enums;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.GoodsReceival
{
    public class PrimeCargoGoodsReceivalCreateFunction
    {
        private readonly IPrimeCargoService primeCargoService;

        public PrimeCargoGoodsReceivalCreateFunction(IPrimeCargoService primeCargoService)
        {
            this.primeCargoService = primeCargoService;
        }

        [FixedDelayRetry(3, "00:05:00")]
        [FunctionName("PrimeCargoGoodsReceivalCreateFunction")]
        [return: ServiceBus("azure-topic-prime-cargo-wms-goods-receival-response", Connection = "serviceBus")]
        public async Task<Message> Run([ServiceBusTrigger("azure-topic-prime-cargo-wms-goods-receival-request", "azure-sub-prime-cargo-goods-receival-create-req", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("PrimeCargoGoodsReceivalCreate function recieved the message from the topic");

                return await this.primeCargoService.CreateOrUpdatePrimeCargoGoodsReceivalAsync(mySbMsg, log, ActionType.Create);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
