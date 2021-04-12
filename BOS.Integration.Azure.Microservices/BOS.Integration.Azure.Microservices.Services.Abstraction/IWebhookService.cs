using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Webhooks;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IWebhookService
    {
        Task<ActionExecutionResult> CreateGoodsReceivalLineCreatedAsync(GoodsReceivalLineCreatedDTO lineCreatedDTO);

        Task<ActionExecutionResult> CreateGoodsReceivalClosedAsync(GoodsReceivalClosedDTO closedDTO);
    }
}
