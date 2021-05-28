using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PrimeCargo;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GoodsReceivalEntity = BOS.Integration.Azure.Microservices.Domain.Entities.GoodsReceival.GoodsReceival;

namespace BOS.Integration.Azure.Microservices.Functions.GoodsReceival
{
    public class GoodsReceivalTimerFunction
    {
        private readonly IPrimeCargoService primeCargoService;
        private readonly IGoodsReceivalService goodsReceivalService;
        private readonly IServiceBusService serviceBusService;
        private readonly ILogService logService;
        private readonly IMapper mapper;

        public GoodsReceivalTimerFunction(
            IPrimeCargoService primeCargoService,
            IServiceBusService serviceBusService,
            IGoodsReceivalService goodsReceivalService,
            ILogService logService,
            IMapper mapper)
        {
            this.primeCargoService = primeCargoService;
            this.serviceBusService = serviceBusService;
            this.goodsReceivalService = goodsReceivalService;
            this.logService = logService;
            this.mapper = mapper;
        }

        [FixedDelayRetry(5, "00:05:00")]
        [FunctionName("GoodsReceivalTimerFunction")]
        public async Task Run([TimerTrigger("0 0 1 * * *")] TimerInfo myTimer, ILogger log)
        {
            try
            {
                log.LogInformation($"GoodsReceivalTimer function executed at: {DateTime.UtcNow}");

                List<Message> messages = new List<Message>();

                var lastUpdate = DateTime.UtcNow;
                bool hasMoreData;

                do
                {
                    // Get goods receivals from Prime Cargo
                    var result = await primeCargoService.GetGoodsReceivalsByLastUpdateAsync(lastUpdate);

                    var content = result.Entity as PrimeCargoResponseContent<List<PrimeCargoGoodsReceivalResponseDTO>>;

                    if (!result.Succeeded || content == null)
                    {
                        string error = result.Error ?? "Could not get GoodsReceivals from PrimeCargo";

                        log.LogError(error);
                        throw new Exception(error);
                    }

                    foreach (var primeCargoGoodsReceival in content.Data)
                    {
                        // Get goods receival from Cosmos DB
                        var goodsReceival = await goodsReceivalService.GetGoodsReceivalByIdAsync(primeCargoGoodsReceival.ReceivalNumber);

                        if (goodsReceival == null)
                        {
                            var createResult = await goodsReceivalService.CreateGoodsReceivalFromPrimeCargoInfoAsync(primeCargoGoodsReceival);

                            goodsReceival = createResult.Entity as GoodsReceivalEntity;

                            if (!createResult.Succeeded || goodsReceival == null)
                            {
                                log.LogError(createResult.Error);
                            }
                            else
                            {
                                LogInfo erpInfo = this.mapper.Map<LogInfo>(goodsReceival);

                                string message = $"The Goods Receival with ReceivalNumber = {goodsReceival.WmsDocumentNo} has not exist yet. A dummy Goods Receival was created in Cosmos DB.";

                                await this.logService.AddTimeLineAsync(erpInfo, message, TimeLineStatus.Error);
                            }
                        }
                        else
                        {
                            LogInfo erpInfo = this.mapper.Map<LogInfo>(goodsReceival);

                            await this.logService.AddTimeLineAsync(erpInfo, NavObject.GoodsReceival + TimeLineDescription.PrimeCargoSuccessfullyReceived, TimeLineStatus.Information);

                            // Update goods receival in Cosmos DB         
                            await goodsReceivalService.UpdateGoodsReceivalFromPrimeCargoInfoAsync(primeCargoGoodsReceival, goodsReceival);

                            log.LogInformation("GoodsReceival is successfully updated in Cosmos DB");

                            // Create a topic message
                            var messageBody = new RequestMessage<GoodsReceivalEntity> { ErpInfo = erpInfo, RequestObject = goodsReceival };

                            messages.Add(serviceBusService.CreateMessage(JsonConvert.SerializeObject(messageBody)));
                        }                        
                    }

                    hasMoreData = content.MoreData.HasValue && content.NextFilterValue.HasValue && content.MoreData.Value;
                    lastUpdate = content.NextFilterValue ?? lastUpdate;
                } while (hasMoreData);

                if (messages.Count > 0)
                {
                    await this.serviceBusService.SendMessagesToTopicAsync("azure-topic-prime-cargo-wms-goods-receival-update", messages);
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
