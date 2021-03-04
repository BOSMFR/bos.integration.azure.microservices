using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.Api
{
    public class GetLogsByObjectIdFunction
    {
        private readonly ILogService logService;

        public GetLogsByObjectIdFunction(ILogService logService)
        {
            this.logService = logService;
        }

        [FunctionName("GetLogsByObjectIdFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetLogsByObjectId")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function - \"GetLogsByObjectIdFunction\" processed a request.");

            if (!req.Query.ContainsKey("id"))
            {
                return new BadRequestObjectResult(new { Error = "The request should contain the \"id\" parameter " });
            }

            string id = req.Query["id"];

            var logs = await this.logService.GetLogsByObjectIdAsync(id);

            return new OkObjectResult(logs);
        }
    }
}
