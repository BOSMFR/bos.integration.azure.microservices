using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GoodsReceivalEntity = BOS.Integration.Azure.Microservices.Domain.Entities.GoodsReceival.GoodsReceival;

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
        public async Task Run([ServiceBusTrigger("azure-topic-prime-cargo-wms-goods-receival-update", "azure-sub-prime-cargo-wms-goods-receival-update", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("NavSetGoodsReceivalClosed function recieved the message from the topic");

                var messageObject = JsonConvert.DeserializeObject<RequestMessage<GoodsReceivalEntity>>(mySbMsg);

                var goodsReceival = messageObject.RequestObject;

                // Set the goods receival closed in Nav
                var result = await navService.UpdateGoodsReceivalIntoNavAsync(goodsReceival.PrimeCargoData);

                var timeLines = (result.Entity as List<TimeLineDTO>) ?? new List<TimeLineDTO>();

                if (!result.Succeeded)
                {
                    log.LogError(result.Error);
                    timeLines.Add(new TimeLineDTO { Status = TimeLineStatus.Error, Description = TimeLineDescription.ErrorClosingGoodsReceival + result.Error, DateTime = DateTime.UtcNow });

                    await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);
                    return;
                }

                timeLines.Add(new TimeLineDTO { Status = TimeLineStatus.Successfully, Description = TimeLineDescription.SuccessfullyClosedGoodsReceival, DateTime = DateTime.UtcNow });

                await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

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
