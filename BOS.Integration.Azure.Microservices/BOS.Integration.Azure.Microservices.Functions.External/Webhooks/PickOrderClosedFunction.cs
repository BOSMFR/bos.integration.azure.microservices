using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Webhooks;
using BOS.Integration.Azure.Microservices.Infrastructure.Configuration;
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

using PickOrderEntity = BOS.Integration.Azure.Microservices.Domain.Entities.PickOrder.PickOrder;

namespace BOS.Integration.Azure.Microservices.Functions.External.Webhooks
{
    public class PickOrderClosedFunction
    {
        private readonly IPrimeCargoService primeCargoService;
        private readonly IServiceBusService serviceBusService;
        private readonly IPickOrderService pickOrderService;
        private readonly IWebhookService webhookService;
        private readonly IConfigurationManager configuration;
        private readonly ILogService logService;
        private readonly IMapper mapper;

        public PickOrderClosedFunction(
            IPrimeCargoService primeCargoService, 
            IServiceBusService serviceBusService,
            IPickOrderService pickOrderService,
            IWebhookService webhookService,
            IConfigurationManager configuration,
            ILogService logService,
            IMapper mapper)
        {
            this.primeCargoService = primeCargoService;
            this.serviceBusService = serviceBusService;
            this.pickOrderService = pickOrderService;
            this.webhookService = webhookService;
            this.configuration = configuration;
            this.logService = logService;
            this.mapper = mapper;
        }

        [FixedDelayRetry(5, "00:05:00")]
        [FunctionName("PickOrderClosedFunction")]
        [return: ServiceBus("azure-topic-prime-cargo-wms-pick-order-update", Connection = "serviceBus")]
        public async Task<Message> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "PickOrderClosed")] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("HTTP trigger function - \"PickOrderClosedFunction\" processed a request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                // Log webhook information
                var webhookInfo = new WebhookInfoDTO { RequestBody = requestBody, Status = TimeLineStatus.Information, Type = WebhookType.PickOrderClosed, Message = "The PickOrderClosed function processed a request" };

                var createWebhookResult = await webhookService.CreateWebhookInfoAsync(webhookInfo);

                if (!createWebhookResult.Succeeded)
                {
                    log.LogError(createWebhookResult.Error);
                    throw new Exception(createWebhookResult.Error);
                }

                // Deserialize and validate request message
                var pickOrderClosedDTO = JsonConvert.DeserializeObject<PickOrderClosedDTO>(requestBody);

                if (string.IsNullOrEmpty(pickOrderClosedDTO.OrderNumber) || pickOrderClosedDTO.PickOrderHeaderId == default)
                {
                    log.LogError("OrderNumber and PickOrderHeaderId properties must have value");
                    return null;
                }

                LogInfo erpInfo;

                // Get pick order from Cosmos DB
                var pickOrder = await pickOrderService.GetPickOrderByIdAsync(pickOrderClosedDTO.OrderNumber);

                if (pickOrder == null)
                {
                    var createResult = await pickOrderService.CreatePickOrderFromPrimeCargoInfoAsync(new PrimeCargoPickOrderResponseDTO { PickOrderHeaderId = pickOrderClosedDTO.PickOrderHeaderId, OrderNumber = pickOrderClosedDTO.OrderNumber });

                    pickOrder = createResult.Entity as PickOrderEntity;

                    if (!createResult.Succeeded || pickOrder == null)
                    {
                        log.LogError(createResult.Error);
                    }
                    else
                    {
                        erpInfo = this.mapper.Map<LogInfo>(pickOrder);

                        string message = $"The Pick Order with OrderNumber = {pickOrder.OrderNumber} has not exist yet. A dummy Pick Order was created in Cosmos DB.";

                        await this.logService.AddTimeLineAsync(erpInfo, message, TimeLineStatus.Error);
                    }

                    return null;
                }

                erpInfo = this.mapper.Map<LogInfo>(pickOrder);

                // Check if the pick order is closed
                if (pickOrder.IsClosed)
                {
                    string message = $"The PickOrder with OrderNumber = {pickOrder.OrderNumber} is already closed";

                    log.LogInformation(message);
                    await this.logService.AddTimeLineAsync(erpInfo, message, TimeLineStatus.Information);

                    return null;
                }

                // Get pick order from Prime Cargo
                string url = configuration.PrimeCargoSettings.Url + "PickOrder/GetPickOrder?pickOrderHeaderId=" + pickOrderClosedDTO.PickOrderHeaderId.ToString();

                var primeCargoPickOrder = await primeCargoService.GetPrimeCargoObjectAsync<PrimeCargoPickOrderResponseDTO>(url, erpInfo, log, NavObject.PickOrder);

                // Update pick order in Cosmos DB            
                await pickOrderService.UpdatePickOrderFromPrimeCargoInfoAsync(primeCargoPickOrder, pickOrder);

                log.LogInformation("PickOrder is successfully updated in Cosmos DB");

                // Create a topic message
                var messageBody = new RequestMessage<PickOrderEntity> { ErpInfo = erpInfo, RequestObject = pickOrder };

                return this.serviceBusService.CreateMessage(JsonConvert.SerializeObject(messageBody));
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }            
        }
    }
}
