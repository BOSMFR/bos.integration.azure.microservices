using AutoMapper;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Webhooks;
using BOS.Integration.Azure.Microservices.Domain.Entities.Webhooks;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class WebhookService : IWebhookService
    {
        private readonly IGoodsReceivalLineCreatedRepository lineCreatedRepository;
        private readonly IWebhookRepository webhookRepository;
        private readonly IMapper mapper;

        public WebhookService(
            IGoodsReceivalLineCreatedRepository lineCreatedRepository,
            IWebhookRepository webhookRepository,
            IMapper mapper)
        {
            this.lineCreatedRepository = lineCreatedRepository;
            this.webhookRepository = webhookRepository;
            this.mapper = mapper;
        }

        public async Task<ActionExecutionResult> CreateWebhookInfoAsync(WebhookInfoDTO webhookInfoDTO)
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                var newWebhook = this.mapper.Map<WebhookInfo>(webhookInfoDTO);

                newWebhook.ReceivedAt = DateTime.Now;

                await webhookRepository.AddAsync(newWebhook, newWebhook.Type);

                actionResult.Entity = newWebhook;
                actionResult.Succeeded = true;
            }
            catch (Exception ex)
            {
                actionResult.Error = ex.Message;
            }

            return actionResult;
        }

        public async Task<ActionExecutionResult> CreateGoodsReceivalLineCreatedAsync(GoodsReceivalLineCreatedDTO lineCreatedDTO)
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                var newWebhook = this.mapper.Map<GoodsReceivalLineCreated>(lineCreatedDTO);

                newWebhook.Type = WebhookType.GoodsReceivalLineCreated;

                await lineCreatedRepository.AddAsync(newWebhook, newWebhook.Type);

                actionResult.Entity = newWebhook;
                actionResult.Succeeded = true;
            }
            catch (Exception ex)
            {
                actionResult.Error = ex.Message;
            }

            return actionResult;
        }
    }
}
