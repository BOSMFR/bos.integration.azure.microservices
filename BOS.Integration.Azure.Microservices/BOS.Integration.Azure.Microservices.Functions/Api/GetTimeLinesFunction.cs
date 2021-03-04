using BOS.Integration.Azure.Microservices.Domain.DTOs;
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
    public class GetTimeLinesFunction
    {
        private readonly ILogService logService;

        public GetTimeLinesFunction(ILogService logService)
        {
            this.logService = logService;
        }

        [FunctionName("GetTimeLinesFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "GetTimeLines")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function - \"GetTimeLinesFunction\" processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var timeLineFilter = JsonConvert.DeserializeObject<TimeLineFilterDTO>(requestBody);

            var timeLines = await logService.GetTimeLinesByFilterAsync(timeLineFilter);

            return new OkObjectResult(timeLines);
        }
    }
}
