using AutoMapper;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Domain.Entities;
using BOS.Integration.Azure.Microservices.Domain.Entities.Product;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions
{
    public class MessageReceiverFunction
    {
        private readonly IProductRepository repository;
        private readonly IMapper mapper;

        public MessageReceiverFunction(IProductRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [FunctionName("MessageReceiverFunction")]
        public async Task Run([ServiceBusTrigger("azure-outbound-engine-dev-add-file", Connection = "serviceBus")] string myQueueItem, IDictionary<string, object> userProperties, ILogger log)
        {
            try
            {
                var messageType = myQueueItem.ToString();

                string category = null;

                if (userProperties.ContainsKey("fileName"))
                {
                    string fileName = userProperties["fileName"].ToString();

                    category = fileName.Split("_").First();
                }

                switch (category)
                {
                    case NavObjectCategory.Sku: log.LogInformation("Sku file was read"); break;
                    case NavObjectCategory.Item:  break;

                    default: log.LogError("Could not recognize the file category as any of known categories"); break;
                }

                //var productDTO = JsonConvert.DeserializeObject<ProductDTO>();

                // var product = this.mapper.Map<Product>(productDTO);

                //Next setps:

                // 1) Check if sku (product) exists in CosmosDB
                // 2) If doesn`t exist -> Create new record in db
                //  2.1) If StartDatePrimeCargoExport <= Now -> Send to PrimeCargo (create message to PC queue)
                //  2.2) Else: End
                // 3) If exists -> Update the record in db

                //repository.GetByIdAsync

                //if (productDTO != null)
                //{
                //    log.LogInformation(productDTO.ToString());

                //    //await repository.AddAsync(obj);
                //}
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
