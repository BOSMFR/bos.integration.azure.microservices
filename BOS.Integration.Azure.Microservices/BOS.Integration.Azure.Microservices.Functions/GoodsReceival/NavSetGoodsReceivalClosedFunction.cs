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
    public class NavSetGoodsReceivalClosedFunction
    {
        private readonly INavService navService;
        private readonly IGoodsReceivalService goodsReceivalService;
        private readonly ILogService logService;

        public NavSetGoodsReceivalClosedFunction(
            INavService navService, 
            IGoodsReceivalService goodsReceivalService, 
            ILogService logService)
        {
            this.navService = navService;
            this.goodsReceivalService = goodsReceivalService;
            this.logService = logService;
        }

        [FixedDelayRetry(5, "00:05:00")]
        [FunctionName("NavSetGoodsReceivalClosedFunction")]
        public async Task Run([ServiceBusTrigger("azure-topic-prime-cargo-wms-goods-receival-closed", "azure-sub-prime-cargo-wms-goods-receival-closed", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("NavSetGoodsReceivalClosed function recieved the message from the topic");

                var messageObject = JsonConvert.DeserializeObject<RequestMessage<PrimeCargoGoodsReceivalResponseDTO>>(mySbMsg);

                // Check if the goods receival is closed
                var goodsReceival = await goodsReceivalService.GetGoodsReceivalByIdAsync(messageObject.RequestObject.ReceivalNumber);

                if (goodsReceival.IsClosed)
                {
                    string message = $"The GoodsReceival with ReceivalNumber = {messageObject.RequestObject.ReceivalNumber} is already closed";

                    log.LogInformation(message);
                    await this.logService.AddTimeLineAsync(messageObject.ErpInfo, message, TimeLineStatus.Information);

                    return;
                }

                // Set the goods receival closed in Nav
                var result = await navService.UpdateGoodsReceivalIntoNavAsync(messageObject.RequestObject);

                if (!result.Succeeded)
                {
                    log.LogError(result.Error);
                    await this.logService.AddTimeLineAsync(messageObject.ErpInfo, TimeLineDescription.ErrorClosingGoodsReceival + result.Error, TimeLineStatus.Error);

                    return;
                }

                await this.logService.AddTimeLineAsync(messageObject.ErpInfo, TimeLineDescription.SuccessfullyClosedGoodsReceival, TimeLineStatus.Successfully);

                // Set the goods receival closed in Cosmos DB
                bool isSucceeded = await goodsReceivalService.SetGoodsReceivalClosedAsync(goodsReceival);

                if (isSucceeded)
                {
                    log.LogInformation("GoodsReceival is successfully updated in Cosmos DB");
                }
                else
                {
                    log.LogError("Could not update the GoodsReceival in Cosmos DB");
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
