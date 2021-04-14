using BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot;
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
    public class GetPackshotsFunction
    {
        private readonly IPackshotService packshotService;

        public GetPackshotsFunction(IPackshotService packshotService)
        {
            this.packshotService = packshotService;
        }

        [FunctionName("GetPackshotsFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "GetPackshots")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function - \"GetPackshotsFunction\" processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var packshotFilter = JsonConvert.DeserializeObject<PackshotFilterDTO>(requestBody);

            var packshots = await this.packshotService.GetPackshotsByFilterAsync(packshotFilter);

            return new OkObjectResult(packshots);
        }
    }
}
