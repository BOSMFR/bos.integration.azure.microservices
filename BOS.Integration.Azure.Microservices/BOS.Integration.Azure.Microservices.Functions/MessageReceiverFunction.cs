using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOS.Integration.Azure.Microservices.Functions
{
    public class MessageReceiverFunction
    {
        [FunctionName("MessageReceiverFunction")]
        [return: ServiceBus("azure-topic-mesage-receiver-from-nav", Connection = "serviceBus")]
        public Message Run([ServiceBusTrigger("azure-queue-outbound-engine", Connection = "serviceBus")] string myQueueItem, IDictionary<string, object> userProperties, ILogger log)
        {
            try
            {   
                string category = this.GetCategory(userProperties);

                byte[] messageBody = Encoding.UTF8.GetBytes(myQueueItem);

                var topicMessage = new Message(messageBody);

                topicMessage.UserProperties.Add("category", category);

                return topicMessage;
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
