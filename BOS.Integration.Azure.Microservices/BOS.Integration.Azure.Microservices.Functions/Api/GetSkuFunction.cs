using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.Api
{
    public class GetSkuFunction
    {
        private readonly IProductService productService;

        public GetSkuFunction(IProductService productService)
        {
            this.productService = productService;
        }

        [FunctionName("GetSkuFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function - \"GetSkuFunction\" processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            return new OkObjectResult(null);
        }
    }
}
