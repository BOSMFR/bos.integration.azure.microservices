namespace BOS.Integration.Azure.Microservices.Infrastructure.Configuration
{
    public class ServiceBusSettings
    {
        public string QueueName { get; set; }

        public string TopicName { get; set; }

        public string SkuSubscription { get; set; }
    }
}
