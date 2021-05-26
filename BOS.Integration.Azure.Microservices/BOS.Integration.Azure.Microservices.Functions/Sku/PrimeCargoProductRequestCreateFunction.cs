using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Domain.Enums;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions
{
    public class PrimeCargoProductRequestCreateFunction
    {
        private readonly IPrimeCargoService primeCargoService;
        private readonly IServiceBusService serviceBusService;
        private readonly IMapper mapper;

        public PrimeCargoProductRequestCreateFunction(
            IPrimeCargoService primeCargoService, 
            IServiceBusService serviceBusService,
            IMapper mapper)
        {
            this.primeCargoService = primeCargoService;
            this.serviceBusService = serviceBusService;
            this.mapper = mapper;
        }

        [FixedDelayRetry(3, "00:05:00")]
        [FunctionName("PrimeCargoProductRequestCreateFunction")]
        [return: ServiceBus("azure-topic-prime-cargo-wms-product-response", Connection = "serviceBus")]
        public async Task<Message> Run([ServiceBusTrigger("azure-topic-prime-cargo-wms-product-request", "azure-sub-prime-cargo-product-create-request", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("PrimeCargoProductRequestCreate function recieved the message from the topic");

                // Deserialize prime cargo request object from the message
                var messageObject = JsonConvert.DeserializeObject<RequestMessage<PrimeCargoProductRequestDTO>>(mySbMsg);

                // Use PrimeCargo API to create a Product
                var response = await this.primeCargoService.CreateOrUpdatePrimeCargoObjectAsync<PrimeCargoProductRequestDTO, PrimeCargoProductResponseData>(messageObject, log, NavObject.Product, ActionType.Create);

                // Map PrimeCargo response to Product response object
                var primeCargoResponse = response != null ? this.mapper.Map<PrimeCargoProductResponseDTO>(response) : new PrimeCargoProductResponseDTO
                {
                    EnaNo = messageObject.RequestObject.Barcode,
                    Success = false
                };

                primeCargoResponse.ErpjobId = messageObject.RequestObject.ErpjobId;

                // Create a topic message
                var messageBody = new ResponseMessage<PrimeCargoProductResponseDTO> { ErpInfo = messageObject.ErpInfo, ResponseObject = primeCargoResponse };

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
