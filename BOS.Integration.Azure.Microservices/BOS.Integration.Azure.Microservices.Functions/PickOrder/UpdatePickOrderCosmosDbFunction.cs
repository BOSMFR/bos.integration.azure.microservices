using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.PickOrder
{
    public class UpdatePickOrderCosmosDbFunction
    {
        private readonly IPickOrderService pickOrderService;

        public UpdatePickOrderCosmosDbFunction(IPickOrderService pickOrderService)
        {
            this.pickOrderService = pickOrderService;
        }

        [FunctionName("UpdatePickOrderCosmosDbFunction")]
        public async Task Run([ServiceBusTrigger("azure-topic-prime-cargo-wms-pick-order-response", "azure-sub-prime-cargo-pick-order-response-db", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("UpdatePickOrderCosmosDb function recieved the message from the topic");

                bool isSucceeded = false;

                var messageObject = JsonConvert.DeserializeObject<ResponseMessage<PrimeCargoPickOrderResponseDTO>>(mySbMsg);

                isSucceeded = await pickOrderService.UpdatePickOrderFromPrimeCargoInfoAsync(messageObject.ResponseObject);

                if (isSucceeded)
                {
                    log.LogInformation("PickOrder is successfully updated in Cosmos DB");
                }
                else
                {
                    log.LogError($"Could not update the PickOrder in Cosmos DB. The PickOrder with id = \"{messageObject.ResponseObject.OrderNumber}\" does not exist.");
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
