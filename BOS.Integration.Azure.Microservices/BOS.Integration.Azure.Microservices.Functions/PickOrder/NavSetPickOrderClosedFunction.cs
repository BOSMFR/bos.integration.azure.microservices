using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PickOrderEntity = BOS.Integration.Azure.Microservices.Domain.Entities.PickOrder.PickOrder;

namespace BOS.Integration.Azure.Microservices.Functions.PickOrder
{
    public class NavSetPickOrderClosedFunction
    {
        private readonly INavService navService;
        private readonly IPickOrderService pickOrderService;
        private readonly ILogService logService;

        public NavSetPickOrderClosedFunction(
            INavService navService,
            IPickOrderService pickOrderService,
            ILogService logService)
        {
            this.navService = navService;
            this.pickOrderService = pickOrderService;
            this.logService = logService;
        }

        [FixedDelayRetry(5, "00:05:00")]
        [FunctionName("NavSetPickOrderClosedFunction")]
        public async Task Run([ServiceBusTrigger("azure-topic-prime-cargo-wms-pick-order-update", "azure-sub-prime-cargo-wms-pick-order-update", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("NavSetPickOrderClosed function recieved the message from the topic");

                var messageObject = JsonConvert.DeserializeObject<RequestMessage<PickOrderEntity>>(mySbMsg);

                var pickOrder = messageObject.RequestObject;

                // Set the pick order closed in Nav
                var result = await navService.UpdatePickOrderIntoNavAsync(pickOrder.PrimeCargoData);

                var timeLines = (result.Entity as List<TimeLineDTO>) ?? new List<TimeLineDTO>();

                if (!result.Succeeded)
                {
                    log.LogError(result.Error);
                    timeLines.Add(new TimeLineDTO { Status = TimeLineStatus.Error, Description = TimeLineDescription.ErrorClosingPickOrder + result.Error, DateTime = DateTime.UtcNow });

                    await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

                    return;
                }

                timeLines.Add(new TimeLineDTO { Status = TimeLineStatus.Successfully, Description = TimeLineDescription.SuccessfullyClosedPickOrder, DateTime = DateTime.UtcNow });

                await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

                // Set the pick order closed in Cosmos DB
                bool isSucceeded = await pickOrderService.SetPickOrderClosedAsync(pickOrder);

                if (isSucceeded)
                {
                    log.LogInformation("PickOrder is successfully updated in Cosmos DB");
                }
                else
                {
                    log.LogError("Could not update the PickOrder in Cosmos DB");
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
