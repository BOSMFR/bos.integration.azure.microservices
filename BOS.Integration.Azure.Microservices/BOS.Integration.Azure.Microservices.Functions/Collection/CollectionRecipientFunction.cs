﻿using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Collection;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.Collection
{
    public class CollectionRecipientFunction
    {
        private readonly ILogService logService;
        private readonly ICollectionService collectionService;
        private readonly IPlytixService plytixService;
        private readonly IMapper mapper;
        private readonly IBlobService blobService;

        public CollectionRecipientFunction(
            ICollectionService collectionService,
            IPlytixService plytixService,
            IMapper mapper,
            ILogService logService,
            IBlobService blobService)
        {
            this.collectionService = collectionService;
            this.plytixService = plytixService;
            this.logService = logService;
            this.mapper = mapper;
            this.blobService = blobService;
        }

        [FixedDelayRetry(3, "00:05:00")]
        [FunctionName("CollectionRecipientFunction")]
        public async Task Run([ServiceBusTrigger("azure-topic-mesage-receiver-from-nav", "azure-collection-recipient-subscription", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("CollectionRecipient function recieved the message from the topic");

                var timeLines = new List<TimeLineDTO>();

                // Read file from blob storage
                string fileContent = await this.blobService.DownloadFileByFileNameAsync(mySbMsg);

                // Get collection object from the topic message and create or update it in the storage
                var collectionDTO = JsonConvert.DeserializeObject<CollectionDTO>(fileContent);

                var collection = await this.collectionService.CreateOrUpdateCollectionAsync(collectionDTO);

                //Create an erp message and time line
                var erpInfo = this.mapper.Map<LogInfo>(collection);

                await this.logService.AddErpMessageAsync(erpInfo, ErpMessageStatus.ReceivedFromErp);
                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.ErpMessageReceived, Status = TimeLineStatus.Information, DateTime = DateTime.Now });

                // Write time lines to database
                await this.logService.AddTimeLinesAsync(erpInfo, timeLines);

                // Update plytix product attribute
                await this.plytixService.UpdateCollectionProductAttributeAsync(collection);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
