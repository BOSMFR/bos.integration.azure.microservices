using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.PickOrder
{
    public class UpdatePickOrderNavFunction
    {
        private readonly INavService navService;
        private readonly ILogService logService;

        public UpdatePickOrderNavFunction(INavService navService, ILogService logService)
        {
            this.navService = navService;
            this.logService = logService;
        }

        [FixedDelayRetry(3, "00:05:00")]
        [FunctionName("UpdatePickOrderNavFunction")]
        public async Task Run([ServiceBusTrigger("azure-topic-prime-cargo-wms-pick-order-response", "azure-sub-prime-cargo-pick-order-response-nav", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("UpdatePickOrderNav function recieved the message from the topic");

                var timeLines = new List<TimeLineDTO>();

                var messageObject = JsonConvert.DeserializeObject<ResponseMessage<PrimeCargoPickOrderResponseDTO>>(mySbMsg);

                ActionExecutionResult result = null;

                if (messageObject?.ResponseObject != null)
                {
                    result = await this.navService.UpdatePickOrderIntoNavAsync(messageObject.ResponseObject);

                    timeLines = result.Entity as List<TimeLineDTO>;
                }

                if (result == null || !result.Succeeded)
                {
                    string errorMessage = string.IsNullOrEmpty(result?.Error) ? "Could not update the PickOrder into Nav" : result.Error;

                    timeLines.Add(new TimeLineDTO { Status = TimeLineStatus.Error, Description = TimeLineDescription.ErrorUpdatingPickOrder + errorMessage, DateTime = DateTime.UtcNow });

                    await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);
                    throw new Exception(errorMessage);
                }

                timeLines.Add(new TimeLineDTO { Status = TimeLineStatus.Successfully, Description = TimeLineDescription.SuccessfullyUpdatedPickOrder, DateTime = DateTime.UtcNow });
                await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
