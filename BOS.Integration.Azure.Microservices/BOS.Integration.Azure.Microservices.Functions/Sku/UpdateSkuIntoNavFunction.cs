using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

                var timeLines = new List<TimeLineDTO>();

                var messageObject = JsonConvert.DeserializeObject<ResponseMessage<PrimeCargoProductResponseDTO>>(mySbMsg);
                var primeCargoResponse = messageObject?.ResponseObject;

                ActionExecutionResult result = null;

                if (!string.IsNullOrEmpty(primeCargoResponse?.EnaNo))
                {
                    result = await this.navService.UpdateSkuIntoNavAsync(primeCargoResponse.EnaNo, primeCargoResponse.ProductId?.ToString() ?? "0");

                    timeLines = result.Entity as List<TimeLineDTO>;
                }

                if (result == null || !result.Succeeded)
                {
                    string errorMessage = string.IsNullOrEmpty(result?.Error) ? "Could not update the sku into Nav" : result.Error;

                    timeLines.Add(new TimeLineDTO { Status = TimeLineStatus.Error, Description = TimeLineDescription.ErrorUpdatingERP + errorMessage, DateTime = DateTime.UtcNow });

                    await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);
                    throw new Exception(errorMessage);
                }

                timeLines.Add(new TimeLineDTO { Status = TimeLineStatus.Successfully, Description = TimeLineDescription.ErpUpdatedSuccessfully, DateTime = DateTime.UtcNow });
                await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

                log.LogInformation("Sku is successfully updated in Nav");
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
