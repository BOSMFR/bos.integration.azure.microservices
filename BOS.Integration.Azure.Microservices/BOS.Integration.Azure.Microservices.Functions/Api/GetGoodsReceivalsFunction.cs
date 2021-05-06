using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
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
    public class GetGoodsReceivalsFunction
    {
        private readonly IGoodsReceivalService goodsReceivalService;

        public GetGoodsReceivalsFunction(IGoodsReceivalService goodsReceivalService)
        {
            this.goodsReceivalService = goodsReceivalService;
        }

        [FunctionName("GetGoodsReceivalsFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "GetGoodsReceivals")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function - \"GetGoodsReceivalsFunction\" processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var goodsReceivalFilter = JsonConvert.DeserializeObject<GoodsReceivalFilterDTO>(requestBody);

            var goodsReceivals = await this.goodsReceivalService.GetGoodsReceivalsByFilterAsync(goodsReceivalFilter);

            return new OkObjectResult(goodsReceivals);
        }
    }
}
