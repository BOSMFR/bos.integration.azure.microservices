using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PrimeCargo;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.GoodsReceival
{
    public class GoodsReceivalTimerFunction
    {
        private readonly IPrimeCargoService primeCargoService;
        private readonly IGoodsReceivalService goodsReceivalService;

        public GoodsReceivalTimerFunction(IPrimeCargoService primeCargoService, IGoodsReceivalService goodsReceivalService)
        {
            this.primeCargoService = primeCargoService;
            this.goodsReceivalService = goodsReceivalService;
        }

        [FixedDelayRetry(5, "00:05:00")]
        [FunctionName("GoodsReceivalTimerFunction")]
        public async Task<List<Message>> Run([TimerTrigger("0 0 1 * * *")] TimerInfo myTimer, ILogger log)
        {
            try
            {
                log.LogInformation($"GoodsReceivalTimer function executed at: {DateTime.Now}");

                List<Message> messages = new List<Message>();

                // Get goods receivals from Prime Cargo
                var result = await primeCargoService.GetGoodsReceivalsByLastUpdateAsync(DateTime.Now);

                var content = result.Entity as PrimeCargoResponseContent<List<PrimeCargoGoodsReceivalResponseDTO>>;

                if (!result.Succeeded || content == null)
                {
                    string error = result.Error ?? "Could not get GoodsReceivals from PrimeCargo";

                    log.LogError(error);
                    throw new Exception(error);
                }

                foreach (var primeCargoGoodsReceival in content.Data)
                {
                    // Update goods receival in Cosmos DB            
                    bool isSucceeded = await goodsReceivalService.UpdateGoodsReceivalFromPrimeCargoInfoAsync(primeCargoGoodsReceival);

                    if (isSucceeded)
                    {
                        log.LogInformation("GoodsReceival is successfully updated in Cosmos DB");
                    }
                    else
                    {
                        log.LogError($"Could not update the GoodsReceival in Cosmos DB. The GoodsReceival with id = \"{primeCargoGoodsReceival.ReceivalNumber}\" does not exist.");
                        return null;
                    }

                    // Generate message add to response list
                    // ToDo
                }

                return null; // return messages; // Temporary
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
