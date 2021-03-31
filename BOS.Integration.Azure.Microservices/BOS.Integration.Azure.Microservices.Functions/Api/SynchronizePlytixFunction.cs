using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Plytix;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.Api
{
    public class SynchronizePlytixFunction
    {
        private readonly IPlytixService plytixService;
        private readonly ILogService logService;
        private readonly IMapper mapper;
        private readonly ICollectionService collectionService;
        private readonly IDeliveryPeriodService deliveryPeriodService;

        public SynchronizePlytixFunction(
            IPlytixService plytixService,
            ILogService logService,
            IMapper mapper,
            ICollectionService collectionService,
            IDeliveryPeriodService deliveryPeriodService)
        {
            this.plytixService = plytixService;
            this.logService = logService;
            this.mapper = mapper;
            this.collectionService = collectionService;
            this.deliveryPeriodService = deliveryPeriodService;
        }

        [FunctionName("SynchronizePlytixFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "SynchronizePlytix")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function - \"SynchronizePlytixFunction\" processed a request.");

            var collectionTimeLines = new List<TimeLineDTO>();
            var collectionErpMessages = new List<string>();

            var deliveryPeriodTimeLines = new List<TimeLineDTO>();
            var deliveryPeriodErpMessages = new List<string>();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var syncRequestDto = JsonConvert.DeserializeObject<PlytixSyncRequestDTO>(requestBody);

            if (string.IsNullOrEmpty(syncRequestDto?.UserName))
            {
                return new BadRequestObjectResult("Username should be specified.");
            }

            // Get collection options
            var collection = await this.collectionService.GetCollectionAsync();
            var collectionOptions = collection.Details.Where(x => x.ShowExternal).Select(x => x.Id);

            var collectionErpInfo = this.mapper.Map<LogInfo>(collection);

            // Get delivery period options
            var deliveryPeriod = await this.deliveryPeriodService.GetDeliveryPeriodAsync();
            var deliveryPeriodOptions = deliveryPeriod.Details.Where(x => x.Active).Select(x => x.Id);

            var deliveryPeriodErpInfo = this.mapper.Map<LogInfo>(deliveryPeriod);

            // Synchronize collection and delivery period options
            var result = await this.plytixService.SynchronizePlytixOptionsAsync(collectionOptions, deliveryPeriodOptions);

            // Add erp messages
            collectionErpMessages.Add(result.UpdateCollectionResult.Succeeded ? ErpMessageStatus.CollectionUpdatedSuccessfully : ErpMessageStatus.CollectionUpdateError);
            deliveryPeriodErpMessages.Add(result.UpdateDeliveryPeriodResult.Succeeded ? ErpMessageStatus.DeliveryPeriodUpdatedSuccessfully : ErpMessageStatus.DeliveryPeriodUpdateError);

            collectionErpMessages.Add(result.GeneralResult.Succeeded ? ErpMessageStatus.PlytixSyncSuccessfully : ErpMessageStatus.PlytixSyncError);
            deliveryPeriodErpMessages.Add(result.GeneralResult.Succeeded ? ErpMessageStatus.PlytixSyncSuccessfully : ErpMessageStatus.PlytixSyncError);

            // Add time lines
            var updateCollectionTimeLines = result.UpdateCollectionResult.Entity as List<TimeLineDTO>;

            if (updateCollectionTimeLines != null)
            {
                collectionTimeLines.AddRange(updateCollectionTimeLines);
            }

            var updateDeliveryPeriodTimeLines = result.UpdateDeliveryPeriodResult.Entity as List<TimeLineDTO>;

            if (updateDeliveryPeriodTimeLines != null)
            {
                deliveryPeriodTimeLines.AddRange(updateDeliveryPeriodTimeLines);
            }

            if (result.GeneralResult.Succeeded)
            {
                var successSyncTimeLine = new TimeLineDTO { Description = TimeLineDescription.PlytixSyncSuccessfully + syncRequestDto.UserName, Status = TimeLineStatus.Successfully, DateTime = DateTime.Now };
                collectionTimeLines.Add(successSyncTimeLine);
                deliveryPeriodTimeLines.Add(successSyncTimeLine);
            }
            else
            {
                var errorSyncTimeLine = new TimeLineDTO { Description = TimeLineDescription.PlytixSyncError + syncRequestDto.UserName, Status = TimeLineStatus.Error, DateTime = DateTime.Now };
                collectionTimeLines.Add(errorSyncTimeLine);
                deliveryPeriodTimeLines.Add(errorSyncTimeLine);
            }

            // Write erp messages and time lines to database
            await this.logService.AddErpMessagesAsync(collectionErpInfo, collectionErpMessages);
            await this.logService.AddTimeLinesAsync(collectionErpInfo, collectionTimeLines);

            await this.logService.AddErpMessagesAsync(deliveryPeriodErpInfo, deliveryPeriodErpMessages);
            await this.logService.AddTimeLinesAsync(deliveryPeriodErpInfo, deliveryPeriodTimeLines);

            if (!result.GeneralResult.Succeeded)
            {
                return new BadRequestObjectResult(result);
            }

            return new OkObjectResult(result);
        }
    }
}
