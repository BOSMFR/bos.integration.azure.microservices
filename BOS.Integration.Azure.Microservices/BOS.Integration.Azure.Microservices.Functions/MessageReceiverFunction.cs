using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions
{
    public class MessageReceiverFunction
    {
        private readonly ICustomerRepository repository;

        public MessageReceiverFunction(ICustomerRepository repository)
        {
            this.repository = repository;
        }

        [FunctionName("MessageReceiverFunction")]
        public async Task Run([ServiceBusTrigger("%ServiceBusQueueName%", Connection = "serviceBus")] string myQueueItem, ILogger log)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<Customer>(myQueueItem);

                if (obj != null)
                {
                    log.LogInformation(obj.ToString());

                    await repository.AddAsync(obj);
                }

            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
            }
        }
    }
}
