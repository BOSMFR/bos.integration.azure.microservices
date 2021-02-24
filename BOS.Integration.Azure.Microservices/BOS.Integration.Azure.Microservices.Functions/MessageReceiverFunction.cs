using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BOS.Integration.Azure.Microservices.Functions
{
    public class MessageReceiverFunction
    {
        private readonly IServiceBusService serviceBusService;

        public MessageReceiverFunction(IServiceBusService serviceBusService)
        {
            this.serviceBusService = serviceBusService;
        }

        [FunctionName("MessageReceiverFunction")]
        [return: ServiceBus("azure-topic-mesage-receiver-from-nav", Connection = "serviceBus")]
        public Message Run([ServiceBusTrigger("azure-queue-outbound-engine", Connection = "serviceBus")] string myQueueItem, IDictionary<string, object> userProperties, ILogger log)
        {
            try
            {
                log.LogInformation("MessageReceiver function recieved the message from the queue");

                string category = this.GetCategory(userProperties);

                var messageProperties = new Dictionary<string, object> { { "category", category } };

                return this.serviceBusService.CreateMessage(myQueueItem, messageProperties);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }

        private string GetCategory(IDictionary<string, object> userProperties)
        {
            string category = null;

            if (userProperties != null)
            {
                if (userProperties.ContainsKey("category"))
                {
                    category = userProperties["category"].ToString();
                }
                else if (userProperties.ContainsKey("fileName"))
                {
                    string fileName = userProperties["fileName"].ToString();

                    category = fileName.Split("_").First();
                }
            }

            return category;
        }
    }
}
