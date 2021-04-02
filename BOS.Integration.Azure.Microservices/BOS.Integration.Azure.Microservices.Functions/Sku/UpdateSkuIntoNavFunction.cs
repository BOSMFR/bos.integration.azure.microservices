using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PrimeCargo;
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
        private readonly ILogService logService;

        public UpdateSkuIntoNavFunction(INavService navService, ILogService logService)
        {
            this.navService = navService;
            this.logService = logService;
        }

        [FunctionName("UpdateSkuIntoNavFunction")]
        public async Task Run([ServiceBusTrigger("azure-topic-prime-cargo-wms-product-response", "azure-sub-prime-cargo-product-response-nav", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("UpdateSkuIntoNav function recieved the message from the topic");

                var messageObject = JsonConvert.DeserializeObject<PrimeCargoResponseMessage<PrimeCargoProductResponseDTO>>(mySbMsg);
                var primeCargoResponse = messageObject?.PrimeCargoResponseObject;

                ActionExecutionResult result = null;

                if (!string.IsNullOrEmpty(primeCargoResponse?.EnaNo) && !string.IsNullOrEmpty(primeCargoResponse?.ProductId?.ToString()))
                {
                    result = await this.navService.UpdateSkuIntoNavAsync(primeCargoResponse.EnaNo, primeCargoResponse.ProductId.ToString());
                }

                if (result == null || !result.Succeeded)
                {
                    string errorMessage = string.IsNullOrEmpty(result?.Error) ? "Could not update the sku into Nav" : result.Error;

                    await this.logService.AddTimeLineAsync(messageObject.ErpInfo, TimeLineDescription.ErrorUpdatingERP + errorMessage, TimeLineStatus.Error);
                    throw new Exception(errorMessage);
                }

                await this.logService.AddTimeLineAsync(messageObject.ErpInfo, TimeLineDescription.ErpUpdatedSuccessfully, TimeLineStatus.Successfully);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
