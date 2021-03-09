using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.Api
{
    public class GetShopsFunction
    {
        private readonly IShopService shopService;

        public GetShopsFunction(IShopService shopService)
        {
            this.shopService = shopService;
        }

        [FunctionName("GetShopsFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetShops")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function - \"GetShopsFunction\" processed a request.");

            string brand = null;

            if (req.Query.ContainsKey("brand"))
            {
                brand = req.Query["brand"];
            }

            var shops = await this.shopService.GetAllActiveAsync(brand);

            return new OkObjectResult(shops);
        }
    }
}
