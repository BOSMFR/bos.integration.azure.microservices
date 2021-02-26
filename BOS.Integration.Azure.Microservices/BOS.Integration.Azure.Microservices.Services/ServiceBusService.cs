using BOS.Integration.Azure.Microservices.Infrastructure.Configuration;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.ServiceBus;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class ServiceBusService : IServiceBusService
    {
        private readonly IConfigurationManager configuration;

        public ServiceBusService(IConfigurationManager configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendMessagesToTopicAsync(string topicName, IList<Message> messages)
        {
            var client = new TopicClient(configuration.ServiceBusConnectionString, topicName);

            await client.SendAsync(messages);

            await client.CloseAsync();
        }

        public Message CreateMessage(string messageStr, IDictionary<string, object> userProperties = null)
        {
            byte[] messageBody = Encoding.UTF8.GetBytes(messageStr);

            var topicMessage = new Message(messageBody);

            if (userProperties?.Count > 0)
            {
                foreach (var property in userProperties)
                {
                    topicMessage.UserProperties.Add(property.Key, property.Value);
                }
            }

            return topicMessage;
        }
    }
}
