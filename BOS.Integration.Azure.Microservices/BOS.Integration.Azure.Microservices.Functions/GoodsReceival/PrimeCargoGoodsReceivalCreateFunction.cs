using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using BOS.Integration.Azure.Microservices.Domain.Enums;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.GoodsReceival
{
    public class PrimeCargoGoodsReceivalCreateFunction
    {
        private readonly IPrimeCargoService primeCargoService;
        private readonly IServiceBusService serviceBusService;

        public PrimeCargoGoodsReceivalCreateFunction(IPrimeCargoService primeCargoService, IServiceBusService serviceBusService)
        {
            this.primeCargoService = primeCargoService;
            this.serviceBusService = serviceBusService;
        }

        [FixedDelayRetry(3, "00:05:00")]
        [FunctionName("PrimeCargoGoodsReceivalCreateFunction")]
        [return: ServiceBus("azure-topic-prime-cargo-wms-goods-receival-response", Connection = "serviceBus")]
        public async Task<Message> Run([ServiceBusTrigger("azure-topic-prime-cargo-wms-goods-receival-request", "azure-sub-prime-cargo-goods-receival-create-req", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("PrimeCargoGoodsReceivalCreate function recieved the message from the topic");

                // Deserialize prime cargo request object from the message
                var messageObject = JsonConvert.DeserializeObject<RequestMessage<PrimeCargoGoodsReceivalRequestDTO>>(mySbMsg);

                // Use PrimeCargo API to create a GoodsReceival
                var response = await this.primeCargoService.CreateOrUpdatePrimeCargoObjectAsync<PrimeCargoGoodsReceivalRequestDTO, PrimeCargoGoodsReceivalResponseDTO>(messageObject, log, NavObject.GoodsReceival, ActionType.Create);

                if (response?.Data == null)
                { 
                    log.LogError("Prime cargo GoodsReceival response object is empty");
                    return null;
                }

                // Create a topic message
                var messageBody = new ResponseMessage<PrimeCargoGoodsReceivalResponseDTO> { ErpInfo = messageObject.ErpInfo, ResponseObject = response.Data };

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
