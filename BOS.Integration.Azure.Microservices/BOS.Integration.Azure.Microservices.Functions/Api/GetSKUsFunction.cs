using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.Api
{
    public class GetSKUsFunction
    {
        private readonly IProductService productService;

        public GetSKUsFunction(IProductService productService)
        {
            this.productService = productService;
        }

        [FunctionName("GetSKUsFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "GetSKUs")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function - \"GetSKUsFunction\" processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var productFilter = JsonConvert.DeserializeObject<ProductFilterDTO>(requestBody);

            var products = await this.productService.GetProductByFilterAsync(productFilter);

            return new OkObjectResult(products);
        }
    }
}
