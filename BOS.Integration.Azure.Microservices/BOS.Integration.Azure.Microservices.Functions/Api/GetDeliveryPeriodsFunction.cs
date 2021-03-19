using BOS.Integration.Azure.Microservices.Domain.DTOs.DeliveryPeriod;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.Api
{
    public class GetDeliveryPeriodsFunction
    {
        private readonly IDeliveryPeriodService deliveryPeriodService;
        private readonly ILogService logService;

        public GetDeliveryPeriodsFunction(IDeliveryPeriodService deliveryPeriodService, ILogService logService)
        {
            this.deliveryPeriodService = deliveryPeriodService;
            this.logService = logService;
        }

        [FunctionName("GetDeliveryPeriodsFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetDeliveryPeriods")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function - \"GetDeliveryPeriodsFunction\" processed a request.");

            var result = new DeliveryPeriodLogsDTO();
             
            result.DeliveryPeriod = await deliveryPeriodService.GetDeliveryPeriodAsync();

            if (result.DeliveryPeriod == null)
            {
                return new NotFoundObjectResult("Could not find any delivery periods");
            }

            result.Logs = await logService.GetLogsByObjectIdAsync(result.DeliveryPeriod.Id);

            return new OkObjectResult(result);
        }
    }
}
