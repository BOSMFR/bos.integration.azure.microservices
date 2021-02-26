using Microsoft.Azure.ServiceBus;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IServiceBusService
    {
        Task SendMessagesToTopicAsync(string topicName, IList<Message> messages);

        Message CreateMessage(string messageStr, IDictionary<string, object> userProperties = null);
    }
}
