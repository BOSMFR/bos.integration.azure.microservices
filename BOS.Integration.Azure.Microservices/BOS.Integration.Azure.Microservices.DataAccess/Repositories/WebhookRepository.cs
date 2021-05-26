using BOS.Integration.Azure.Microservices.DataAccess.Abstraction;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities.Webhooks;
using Microsoft.Azure.Cosmos;
using System;

namespace BOS.Integration.Azure.Microservices.DataAccess.Repositories
{
    public class WebhookRepository : CosmosDbRepository<WebhookInfo>, IWebhookRepository
    {
        public override string ContainerName { get; } = "webhook";

        public override string GenerateId(WebhookInfo entity) => Guid.NewGuid().ToString();

        public override PartitionKey ResolvePartitionKey(string partitionKey) => new PartitionKey(partitionKey);

        public WebhookRepository(ICosmosDbContainerFactory factory)
            : base(factory)
        {
        }
    }
}
