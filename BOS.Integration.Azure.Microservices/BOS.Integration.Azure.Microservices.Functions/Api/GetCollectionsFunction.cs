using BOS.Integration.Azure.Microservices.Domain.DTOs.Collection;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.Api
{
    public class GetCollectionsFunction
    {
        private readonly ICollectionService collectionService;
        private readonly ILogService logService;

        public GetCollectionsFunction(ICollectionService collectionService, ILogService logService)
        {
            this.collectionService = collectionService;
            this.logService = logService;
        }

        [FunctionName("GetCollectionsFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetCollections")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function - \"GetCollectionsFunction\" processed a request.");

            var result = new CollectionLogsDTO();

            result.Collection = await collectionService.GetCollectionAsync();

            if (result.Collection == null)
            {
                return new NotFoundObjectResult("Could not find any collections");
            }

            result.Logs = await logService.GetLogsByObjectIdAsync(result.Collection.Id);

            return new OkObjectResult(result);
        }
    }
}
