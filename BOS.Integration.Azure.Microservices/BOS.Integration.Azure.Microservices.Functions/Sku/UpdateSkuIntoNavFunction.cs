using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions
{
    public class UpdateSkuIntoNavFunction
    {
        private readonly INavService navService;

        public UpdateSkuIntoNavFunction(INavService navService)
        {
            this.navService = navService;
        }

        [FunctionName("UpdateSkuIntoNavFunction")]
        public async Task Run([ServiceBusTrigger("azure-topic-prime-cargo-wms-product-response", "azure-sub-prime-cargo-product-response-nav", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("UpdateSkuIntoNav function recieved the message from the topic");

                var messageObject = JsonConvert.DeserializeObject<PrimeCargoProductResponseMessage>(mySbMsg);
                var primeCargoResponse = messageObject?.PrimeCargoProduct;

                ActionExecutionResult result = null;

                if (!string.IsNullOrEmpty(primeCargoResponse?.EnaNo) && !string.IsNullOrEmpty(primeCargoResponse?.ProductId?.ToString()))
                {
                    result = await this.navService.UpdateSkuIntoNavAsync(primeCargoResponse.EnaNo, primeCargoResponse.ProductId.ToString());
                }

                if (result == null || !result.Succeeded)
                {
                    throw new Exception("Could not update the sku into Nav");
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
