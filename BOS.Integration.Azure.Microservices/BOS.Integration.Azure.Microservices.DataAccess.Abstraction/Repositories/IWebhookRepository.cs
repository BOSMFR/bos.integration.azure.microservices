using BOS.Integration.Azure.Microservices.Domain.Entities.Webhooks;

namespace BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories
{
    public interface IWebhookRepository : IRepository<WebhookInfo>
    {
    }
}
