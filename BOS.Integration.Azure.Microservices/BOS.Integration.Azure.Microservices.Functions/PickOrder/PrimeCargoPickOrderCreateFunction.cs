using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder;
using BOS.Integration.Azure.Microservices.Domain.Enums;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.PickOrder
{
    public class PrimeCargoPickOrderCreateFunction
    {
        private readonly IPrimeCargoService primeCargoService;
        private readonly IServiceBusService serviceBusService;

        public PrimeCargoPickOrderCreateFunction(IPrimeCargoService primeCargoService, IServiceBusService serviceBusService)
        {
            this.primeCargoService = primeCargoService;
            this.serviceBusService = serviceBusService;
        }

        [FixedDelayRetry(3, "00:05:00")]
        [FunctionName("PrimeCargoPickOrderCreateFunction")]
        [return: ServiceBus("azure-topic-prime-cargo-wms-pick-order-response", Connection = "serviceBus")]
        public async Task<Message> Run([ServiceBusTrigger("azure-topic-prime-cargo-wms-pick-order-request", "azure-sub-prime-cargo-pick-order-create-req", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("PrimeCargoPickOrderCreate function recieved the message from the topic");

                // Deserialize prime cargo request object from the message
                var messageObject = JsonConvert.DeserializeObject<RequestMessage<PrimeCargoPickOrderRequestDTO>>(mySbMsg);

                // Use PrimeCargo API to create a PickOrder
                var response = await this.primeCargoService.CreateOrUpdatePrimeCargoObjectAsync<PrimeCargoPickOrderRequestDTO, PrimeCargoPickOrderResponseDTO>(messageObject, log, NavObject.PickOrder, ActionType.Create);

                if (response?.Data == null)
                {
                    log.LogError("Prime cargo PickOrder response object is empty");
                    return null;
                }

                // Create a topic message
                var messageBody = new ResponseMessage<PrimeCargoPickOrderResponseDTO> { ErpInfo = messageObject.ErpInfo, ResponseObject = response.Data };

                string primeCargoProductResponseJson = JsonConvert.SerializeObject(messageBody);

                return this.serviceBusService.CreateMessage(primeCargoProductResponseJson);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
