using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.Api
{
    public class SynchronizeProductAttributesFunction
    {
        private readonly IPlytixService plytixService;

        public SynchronizeProductAttributesFunction(IPlytixService plytixService)
        {
            this.plytixService = plytixService;
        }

        [FunctionName("SynchronizeProductAttributesFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "SynchronizeProductAttributes")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function - \"SynchronizeProductAttributesFunction\" processed a request.");

            var result = await this.plytixService.SynchronizeProductAttributesAsync();

            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(result);
            }

            return new OkObjectResult(result);
        }
    }
}
