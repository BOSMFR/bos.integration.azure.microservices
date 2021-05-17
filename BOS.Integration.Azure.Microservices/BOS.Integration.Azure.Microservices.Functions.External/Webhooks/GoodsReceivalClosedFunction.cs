using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PrimeCargo;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Webhooks;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.External.Webhooks
{
    public class GoodsReceivalClosedFunction
    {
        private readonly IPrimeCargoService primeCargoService;
        private readonly IServiceBusService serviceBusService;
        private readonly IGoodsReceivalService goodsReceivalService;
        private readonly ILogService logService;
        private readonly IMapper mapper;

        public GoodsReceivalClosedFunction(
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
        [FunctionName("GoodsReceivalClosedFunction")]
        [return: ServiceBus("azure-topic-prime-cargo-wms-goods-receival-closed", Connection = "serviceBus")]
        public async Task<Message> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "GoodsReceivalClosed")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function - \"GoodsReceivalClosedFunction\" processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var goodsReceivalClosedDTO = JsonConvert.DeserializeObject<GoodsReceivalClosedDTO>(requestBody);

            if (string.IsNullOrEmpty(goodsReceivalClosedDTO.ReceivalNumber))
            {
                log.LogError("ReceivalNumber property should have value");
                return null;
            }

            // Get goods receival from Cosmos DB
            var goodsReceival = await goodsReceivalService.GetGoodsReceivalByIdAsync(goodsReceivalClosedDTO.ReceivalNumber);

            if (goodsReceival == null)
            {
                string error = $"Could not find a GoodsReceival with Id = {goodsReceivalClosedDTO.ReceivalNumber} into Cosmos DB";

                log.LogError(error);
                return null;
            }

            LogInfo erpInfo = this.mapper.Map<LogInfo>(goodsReceival);            

            // Get goods receival from Prime Cargo
            var result = await primeCargoService.GetPrimeCargoGoodsReceivalByIdAsync(goodsReceivalClosedDTO.GoodsReceivalId.ToString());

            var requestObject = result.Entity as PrimeCargoResponseContent<PrimeCargoGoodsReceivalResponseDTO>;

            if (!result.Succeeded || requestObject == null)
            {
                string error = result.Error ?? "Could not get a GoodsReceival from PrimeCargo";

                await this.logService.AddTimeLineAsync(erpInfo, TimeLineDescription.ErrorGettingGoodsReceival + error, TimeLineStatus.Error);

                log.LogError(error);
                throw new Exception(error);
            }

            await this.logService.AddTimeLineAsync(erpInfo, TimeLineDescription.SuccessfullyReceivedGoodsReceival, TimeLineStatus.Information);

            // Update goods receival in Cosmos DB            
            bool isSucceeded = await goodsReceivalService.UpdateGoodsReceivalFromPrimeCargoInfoAsync(requestObject.Data);

            if (isSucceeded)
            {
                log.LogInformation("GoodsReceival is successfully updated in Cosmos DB");
            }
            else
            {
                log.LogError("Could not update the GoodsReceival in Cosmos DB");
            }

            // Create a topic message
            var messageBody = new RequestMessage<PrimeCargoGoodsReceivalResponseDTO> { ErpInfo = erpInfo, RequestObject = requestObject.Data };

            return this.serviceBusService.CreateMessage(JsonConvert.SerializeObject(messageBody));
        }
    }
}
