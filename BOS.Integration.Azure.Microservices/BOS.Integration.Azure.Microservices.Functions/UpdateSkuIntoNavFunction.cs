using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions
{
    public class UpdateSkuIntoNavFunction
    {
        [FunctionName("UpdateSkuIntoNavFunction")]
        public async Task Run([ServiceBusTrigger("azure-topic-prime-cargo-wms-product-response", "azure-sub-prime-cargo-product-response-nav", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("UpdateSkuIntoNav function recieved the message from the topic");

                var primeCargoResponse = JsonConvert.DeserializeObject<PrimeCargoProductResponseDTO>(mySbMsg);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
