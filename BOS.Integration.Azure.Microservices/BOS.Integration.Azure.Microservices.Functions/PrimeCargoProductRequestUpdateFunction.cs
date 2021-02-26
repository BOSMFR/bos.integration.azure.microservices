using BOS.Integration.Azure.Microservices.Domain.Constants;
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
    public class PrimeCargoProductRequestUpdateFunction
    {
        private readonly IPrimeCargoService primeCargoService;
        private readonly IValidationService validationService;
        private readonly ILogService logService;
        private readonly IServiceBusService serviceBusService;

        public PrimeCargoProductRequestUpdateFunction(
            IPrimeCargoService primeCargoService, 
            IValidationService validationService,
            ILogService logService,
            IServiceBusService serviceBusService)
        {
            this.primeCargoService = primeCargoService;
            this.validationService = validationService;
            this.logService = logService;
            this.serviceBusService = serviceBusService;
        }


        [FunctionName("PrimeCargoProductRequestUpdateFunction")]
        [return: ServiceBus("azure-topic-prime-cargo-wms-product-response", Connection = "serviceBus")]
        public async Task<Message> Run([ServiceBusTrigger("azure-topic-prime-cargo-wms-product-request", "azure-sub-prime-cargo-product-update-request", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("PrimeCargoProductRequestUpdate function recieved the message from the topic");

                // Deserialize prime cargo product from the message and validate it
                var messageObject = JsonConvert.DeserializeObject<PrimeCargoProductRequestMessage>(mySbMsg);

                await this.logService.AddErpMessageAsync(messageObject.ErpInfo, ErpMessageStatus.UpdateMessage);

                if (!validationService.Validate(messageObject.PrimeCargoProduct))
                {
                    await this.logService.AddErpMessageAsync(messageObject.ErpInfo, ErpMessageStatus.Error);
                    await this.logService.AddTimeLineAsync(messageObject.ErpInfo, TimeLineDescription.DataValidationFailed, TimeLineStatus.Error);
                    log.LogError("Prime Cargo object validation error occured");
                    return null;
                }

                await this.logService.AddTimeLineAsync(messageObject.ErpInfo, TimeLineDescription.ProductUpdateMessageSentServiceBus, TimeLineStatus.Information);

                // Use prime cargo API to update the object
                var response = await this.primeCargoService.CreateOrUpdatePrimeCargoProductAsync(messageObject.PrimeCargoProduct, ActionType.Update);

                if (response.Succeeded)
                {
                    await this.logService.AddErpMessageAsync(messageObject.ErpInfo, ErpMessageStatus.DeliveredSuccessfully);
                    await this.logService.AddTimeLineAsync(messageObject.ErpInfo, TimeLineDescription.DeliveredSuccessfullyToPrimeCargo, TimeLineStatus.Successfully);
                }
                else
                {
                    string errorMessage = string.IsNullOrEmpty(response.Error) ? "Could not update the object via prime cargo API" : response.Error;
                    log.LogError(errorMessage);
                    await this.logService.AddTimeLineAsync(messageObject.ErpInfo, TimeLineDescription.PrimeCargoRequestError + errorMessage, TimeLineStatus.Error);
                }

                var primeCargoResponse = response.Entity as PrimeCargoProductResponseDTO;

                if (primeCargoResponse == null)
                {
                    primeCargoResponse = new PrimeCargoProductResponseDTO
                    {
                        EnaNo = messageObject.PrimeCargoProduct.Barcode,
                        Success = response.Succeeded
                    };
                }

                // Create a topic message
                var messageBody = new PrimeCargoProductResponseMessage { ErpInfo = messageObject.ErpInfo, PrimeCargoProduct = primeCargoResponse };

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
