using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions
{
    public class UpdateSkuIntoCosmosDbFunction
    {
        private readonly IProductService productService;

        public UpdateSkuIntoCosmosDbFunction(IProductService productService)
        {
            this.productService = productService;
        }

        [FunctionName("UpdateSkuIntoCosmosDbFunction")]
        public async Task Run([ServiceBusTrigger("azure-topic-prime-cargo-wms-product-response", "azure-sub-prime-cargo-product-response-cosmos-db", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("UpdateSkuIntoCosmosDb function recieved the message from the topic");

                var primeCargoResponse = JsonConvert.DeserializeObject<PrimeCargoProductResponseDTO>(mySbMsg);

                if (primeCargoResponse != null)
                {
                    await productService.UpdateProductFromPrimeCargoInfoAsync(primeCargoResponse);

                    log.LogInformation("Sku is successfully updated in Cosmos DB");
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
