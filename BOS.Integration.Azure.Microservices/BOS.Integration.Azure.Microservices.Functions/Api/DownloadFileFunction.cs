using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.Api
{
    public class DownloadFileFunction
    {
        private readonly IBlobService blobService;

        public DownloadFileFunction(IBlobService blobService)
        {
            this.blobService = blobService;
        }

        [FunctionName("DownloadFileFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "DownloadFile")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function - \"DownloadFileFunction\" processed a request.");

            if (!req.Query.ContainsKey("fileName"))
            {
                return new BadRequestObjectResult(new { Error = "The request should contain the \"fileName\" parameter " });
            }

            string fileName = req.Query["fileName"];

            var result = await this.blobService.DownloadFileByFileNameAsync(fileName);

            if (result == null || !result.Succeeded)
            {
                return new BadRequestObjectResult(new { Error = result?.Error ?? $"Could not find the file by specified name" });
            }

            var fileDto = result.Entity as DownloadFileDTO;

            return new FileContentResult(fileDto.Content, fileDto.ContentType);
        }
    }
}
