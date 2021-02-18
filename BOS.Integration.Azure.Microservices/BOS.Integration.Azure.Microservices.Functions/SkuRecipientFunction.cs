using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions
{
    public class SkuRecipientFunction
    {
        private const int DescriptionMaxLength = 50;

        private readonly IProductService productService;
        private readonly IMapper mapper;

        public SkuRecipientFunction(IProductService productService, IMapper mapper)
        {
            this.productService = productService;
            this.mapper = mapper;
        }

        [FunctionName("SkuRecipientFunction")]
        [return: ServiceBus("azure-topic-prime-cargo-wms-product-request", Connection = "serviceBus")]
        public async Task<Message> Run([ServiceBusTrigger("azure-topic-mesage-receiver-from-nav", "azure-sku-recipient-subscription", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("SkuRecipient function recieved the message from the topic");

                // Get product objetc from the topic message and create or update it in the storage
                var productDTO = JsonConvert.DeserializeObject<ProductDTO>(mySbMsg);

                string primeCargoIntegrationState = GetPrimeCargoIntegrationState(productDTO.StartDatePrimeCargoExport);

                bool isNewObjectCreated = await this.productService.CreateOrUpdateProductAsync(productDTO, primeCargoIntegrationState);

                if (primeCargoIntegrationState == PrimeCargoIntegrationState.Waiting)
                {
                    log.LogError("Sku is not sent to PrimeCargo because startDatePrimeCargoExport was in wrong format or value was more than today");
                    return null;
                }

                // Map the product to the prime cargo request object and check a description
                var primeCargoProduct = this.mapper.Map<PrimeCargoProductRequestDTO>(productDTO);

                TrimPrimeCargoProductDescription(primeCargoProduct);

                // Create a topic message
                string primeCargoProductJson = JsonConvert.SerializeObject(primeCargoProduct);

                byte[] messageBody = Encoding.UTF8.GetBytes(primeCargoProductJson);

                var topicMessage = new Message(messageBody);

                topicMessage.UserProperties.Add("type", isNewObjectCreated ? "create" : "update");

                return topicMessage;
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }

        private string GetPrimeCargoIntegrationState(string startDatePrimeCargoExport)
            => (!DateTime.TryParseExact(startDatePrimeCargoExport, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate) || startDate > DateTime.Today)
                ? PrimeCargoIntegrationState.Waiting : PrimeCargoIntegrationState.NotDelivered;

        private void TrimPrimeCargoProductDescription(PrimeCargoProductRequestDTO primeCargoProduct)
        {
            primeCargoProduct.Description = primeCargoProduct.Description?.Length > DescriptionMaxLength
                                                ? primeCargoProduct.Description.Substring(0, DescriptionMaxLength).Trim()
                                                : primeCargoProduct.Description;
        }
    }
}
