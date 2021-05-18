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
    public class UpdateGoodsReceivalCosmosDbFunction
    {
        private readonly IGoodsReceivalService goodsReceivalService;

        public UpdateGoodsReceivalCosmosDbFunction(IGoodsReceivalService goodsReceivalService)
        {
            this.goodsReceivalService = goodsReceivalService;
        }

        [FunctionName("UpdateGoodsReceivalCosmosDbFunction")]
        public async Task Run([ServiceBusTrigger("azure-topic-prime-cargo-wms-goods-receival-response", "azure-sub-prime-cargo-goods-receival-response-db", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("UpdateGoodsReceivalCosmosDb function recieved the message from the topic");

                bool isSucceeded = false;

                var messageObject = JsonConvert.DeserializeObject<ResponseMessage<PrimeCargoGoodsReceivalResponseDTO>>(mySbMsg);

                isSucceeded = await goodsReceivalService.UpdateGoodsReceivalFromPrimeCargoInfoAsync(messageObject.ResponseObject);

                if (isSucceeded)
                {
                    log.LogInformation("GoodsReceival is successfully updated in Cosmos DB");
                }
                else
                {
                    log.LogError($"Could not update the GoodsReceival in Cosmos DB. The GoodsReceival with id = \"{messageObject.ResponseObject.ReceivalNumber}\" does not exist.");
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
