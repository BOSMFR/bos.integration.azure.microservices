using BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder;
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
    public class GetPickOrdersFunction
    {
        private readonly IPickOrderService pickOrderService;

        public GetPickOrdersFunction(IPickOrderService pickOrderService)
        {
            this.pickOrderService = pickOrderService;
        }

        [FunctionName("GetPickOrdersFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "GetPickOrders")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function - \"GetPickOrdersFunction\" processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var pickOrderFilter = JsonConvert.DeserializeObject<PickOrderFilterDTO>(requestBody);

            var pickOrders = await this.pickOrderService.GetPickOrdersByFilterAsync(pickOrderFilter);

            return new OkObjectResult(pickOrders);
        }
    }
}
