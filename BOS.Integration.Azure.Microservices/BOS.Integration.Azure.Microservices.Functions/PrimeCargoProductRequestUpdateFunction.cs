using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Infrastructure.Configuration;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions
{
    public class PrimeCargoProductRequestUpdateFunction
    {
        private readonly IConfigurationManager configurationManager;
        private readonly IHttpService httpService;
        private readonly IValidationService validationService;

        public PrimeCargoProductRequestUpdateFunction(IConfigurationManager configurationManager, IHttpService httpService, IValidationService validationService)
        {
            this.configurationManager = configurationManager;
            this.httpService = httpService;
            this.validationService = validationService;
        }


        [FunctionName("PrimeCargoProductRequestUpdateFunction")]
        [return: ServiceBus("azure-topic-prime-cargo-wms-product-request", Connection = "serviceBus")]
        public async Task<Message> Run([ServiceBusTrigger("azure-topic-prime-cargo-wms-product-request", "azure-sub-prime-cargo-product-update-request", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("PrimeCargoProductRequestUpdate function recieved the message from the topic");

                // Deserialize prime cargo product from the message and validate it
                var primeCargoProduct = JsonConvert.DeserializeObject<PrimeCargoProductRequestDTO>(mySbMsg);

                if (!validationService.Validate(primeCargoProduct))
                {
                    log.LogError("Prime Cargo object validation error occured");
                    return null;
                }

                // Use prime cargo API to update the object
                string url = configurationManager.PrimeCargoSettings.Url + "Product/UpdateProduct";

                // ToDo - Call prime cargo API instead of test response
                //var response = await this.httpService.PostAsync<PrimeCargoProductRequestDTO, PrimeCargoProductResponseDTO>(url, primeCargoProduct, configurationManager.PrimeCargoSettings.Key);
                var response = new PrimeCargoProductResponseDTO { EnaNo = primeCargoProduct.Barcode, ErpjobId = primeCargoProduct.ErpjobId, ProductId = 1 };

                // Create a topic message
                string primeCargoProductResponseJson = JsonConvert.SerializeObject(response);

                byte[] messageBody = Encoding.UTF8.GetBytes(primeCargoProductResponseJson);

                var topicMessage = new Message(messageBody);

                return topicMessage;
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
