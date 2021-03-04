using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.Api
{
    public class GetSkuByIdFunction
    {
        private readonly IProductService productService;

        public GetSkuByIdFunction(IProductService productService)
        {
            this.productService = productService;
        }

        [FunctionName("GetSkuByIdFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetSkuById")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function - \"GetSkuByIdFunction\" processed a request.");

            if (!req.Query.ContainsKey("id"))
            {
                return new BadRequestObjectResult(new { Error = "The request should contain the \"id\" parameter " });
            }

            string id = req.Query["id"];

            //var product = await this.productService.GetProductByIdAsync(id);

            return new OkObjectResult(null);
        }
    }
}
