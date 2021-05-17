using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.GoodsReceival
{
    public class UpdateGoodsReceivalNavFunction
    {
        private readonly INavService navService;
        private readonly ILogService logService;

        public UpdateGoodsReceivalNavFunction(INavService navService, ILogService logService)
        {
            this.navService = navService;
            this.logService = logService;
        }

        [FixedDelayRetry(3, "00:05:00")]
        [FunctionName("UpdateGoodsReceivalNavFunction")]
        public async Task Run([ServiceBusTrigger("azure-topic-prime-cargo-wms-goods-receival-response", "azure-sub-prime-cargo-goods-receival-response-nav", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("UpdateGoodsReceivalNav function recieved the message from the topic");

                var messageObject = JsonConvert.DeserializeObject<ResponseMessage<PrimeCargoGoodsReceivalResponseDTO>>(mySbMsg);

                ActionExecutionResult result = null;

                if (messageObject?.ResponseObject != null)
                {
                    result = await this.navService.UpdateGoodsReceivalIntoNavAsync(messageObject.ResponseObject);
                }

                if (result == null || !result.Succeeded)
                {
                    string errorMessage = string.IsNullOrEmpty(result?.Error) ? "Could not update the GoodsReceival into Nav" : result.Error;

                    await this.logService.AddTimeLineAsync(messageObject.ErpInfo, TimeLineDescription.ErrorUpdatingGoodsReceival + errorMessage, TimeLineStatus.Error);
                    throw new Exception(errorMessage);
                }

                await this.logService.AddTimeLineAsync(messageObject.ErpInfo, TimeLineDescription.SuccessfullyUpdatedGoodsReceival, TimeLineStatus.Successfully);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
