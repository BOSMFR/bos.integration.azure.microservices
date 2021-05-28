using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder;
using BOS.Integration.Azure.Microservices.Infrastructure.Configuration;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PickOrderEntity = BOS.Integration.Azure.Microservices.Domain.Entities.PickOrder.PickOrder;

namespace BOS.Integration.Azure.Microservices.Functions.PickOrder
{
    public class PickOrderTimerFunction
    {
        private readonly IPrimeCargoService primeCargoService;
        private readonly IPickOrderService pickOrderService;
        private readonly IServiceBusService serviceBusService;
        private readonly IConfigurationManager configuration;
        private readonly IMapper mapper;

        public PickOrderTimerFunction(
            IPrimeCargoService primeCargoService,
            IServiceBusService serviceBusService,
            IConfigurationManager configuration,
            IPickOrderService pickOrderService,
            IMapper mapper)
        {
            this.primeCargoService = primeCargoService;
            this.serviceBusService = serviceBusService;
            this.configuration = configuration;
            this.pickOrderService = pickOrderService;
            this.mapper = mapper;
        }

        [FixedDelayRetry(5, "00:05:00")]
        [FunctionName("PickOrderTimerFunction")]
        public async Task Run([TimerTrigger("0 0 1 * * *")] TimerInfo myTimer, ILogger log)
        {
            try
            {
                log.LogInformation($"PickOrderTimer function executed at: {DateTime.UtcNow}");

                List<Message> messages = new List<Message>();

                var openPickOrders = await pickOrderService.GetOpenPickOrdersAsync();

                foreach (var pickOrder in openPickOrders)
                {
                    if (pickOrder.PrimeCargoData?.PickOrderHeaderId != null)
                    {
                        var erpInfo = this.mapper.Map<LogInfo>(pickOrder);

                        // Get pick order from Prime Cargo
                        string url = configuration.PrimeCargoSettings.Url + "PickOrder/GetPickOrder?pickOrderHeaderId=" + pickOrder.PrimeCargoData.PickOrderHeaderId.Value.ToString();

                        var primeCargoPickOrder = await primeCargoService.GetPrimeCargoObjectAsync<PrimeCargoPickOrderResponseDTO>(url, erpInfo, log, NavObject.PickOrder);

                        // Update pick order in Cosmos DB
                        await pickOrderService.UpdatePickOrderFromPrimeCargoInfoAsync(primeCargoPickOrder, pickOrder);

                        log.LogInformation("PickOrder is successfully updated in Cosmos DB");

                        // Create a topic message
                        var messageBody = new RequestMessage<PickOrderEntity> { ErpInfo = erpInfo, RequestObject = pickOrder };

                        messages.Add(serviceBusService.CreateMessage(JsonConvert.SerializeObject(messageBody)));
                    }
                }

                if (messages.Count > 0)
                {
                    await this.serviceBusService.SendMessagesToTopicAsync("azure-topic-prime-cargo-wms-pick-order-update", messages);
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
