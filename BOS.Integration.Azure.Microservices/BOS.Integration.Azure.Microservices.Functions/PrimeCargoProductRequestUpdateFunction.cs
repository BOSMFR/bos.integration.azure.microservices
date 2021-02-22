using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Domain.Enums;
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
        private readonly IPrimeCargoService primeCargoService;
        private readonly IValidationService validationService;

        public PrimeCargoProductRequestUpdateFunction(IPrimeCargoService primeCargoService, IValidationService validationService)
        {
            this.primeCargoService = primeCargoService;
            this.validationService = validationService;
        }


        [FunctionName("PrimeCargoProductRequestUpdateFunction")]
        [return: ServiceBus("azure-topic-prime-cargo-wms-product-response", Connection = "serviceBus")]
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
                var primeCargoResponse = await this.primeCargoService.CreateOrUpdatePrimeCargoProductAsync(primeCargoProduct, ActionType.Update);

                if (!primeCargoResponse.Succeeded)
                {
                    string errorMessage = string.IsNullOrEmpty(primeCargoResponse.Error) ? "Could not update the object via prime cargo API" : primeCargoResponse.Error;

                    log.LogError(errorMessage);
                    return null;
                }

                // Create a topic message
                string primeCargoProductResponseJson = JsonConvert.SerializeObject(primeCargoResponse.Entity);

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
