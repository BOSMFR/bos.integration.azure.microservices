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
        private readonly IGoodsReceivalClosedRepository closedRepository;
        private readonly IMapper mapper;

        public WebhookService(
            IGoodsReceivalLineCreatedRepository lineCreatedRepository, 
            IGoodsReceivalClosedRepository closedRepository, 
            IMapper mapper)
        {
            this.lineCreatedRepository = lineCreatedRepository;
            this.closedRepository = closedRepository;
            this.mapper = mapper;
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

        public async Task<ActionExecutionResult> CreateGoodsReceivalClosedAsync(GoodsReceivalClosedDTO closedDTO)
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                var newWebhook = this.mapper.Map<GoodsReceivalClosed>(closedDTO);

                newWebhook.Type = WebhookType.GoodsReceivalClosed;

                await closedRepository.AddAsync(newWebhook, newWebhook.Type);

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
