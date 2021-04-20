using BOS.Integration.Azure.Microservices.Domain.DTOs.Webhooks;
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
    public class GoodsReceivalLineCreatedFunction
    {
        private readonly IWebhookService webhookService;

        public GoodsReceivalLineCreatedFunction(IWebhookService webhookService)
        {
            this.webhookService = webhookService;
        }

        [FunctionName("GoodsReceivalLineCreatedFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "GoodsReceivalLineCreated")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function - \"GoodsReceivalLineCreatedFunction\" processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var goodsReceivalLineCreatedDTO = JsonConvert.DeserializeObject<GoodsReceivalLineCreatedDTO>(requestBody);

            var result = await webhookService.CreateGoodsReceivalLineCreatedAsync(goodsReceivalLineCreatedDTO);

            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(result);
            }

            return new OkObjectResult(result);
        }
    }
}
