using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using BOS.Integration.Azure.Microservices.Services.Helpers;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoodsReceivalEntity = BOS.Integration.Azure.Microservices.Domain.Entities.GoodsReceival.GoodsReceival;

namespace BOS.Integration.Azure.Microservices.Functions.GoodsReceival
{
    public class GoodsReceivalRecipientFunction
    {
        private readonly IGoodsReceivalService goodsReceivalService;
        private readonly IValidationService validationService;
        private readonly IServiceBusService serviceBusService;
        private readonly IBlobService blobService;
        private readonly ILogService logService;
        private readonly IMapper mapper;

        public GoodsReceivalRecipientFunction(
            IGoodsReceivalService goodsReceivalService,
            IValidationService validationService,
            IServiceBusService serviceBusService,
            IBlobService blobService,
            IMapper mapper,
            ILogService logService)
        {
            this.goodsReceivalService = goodsReceivalService;
            this.validationService = validationService;
            this.serviceBusService = serviceBusService;
            this.blobService = blobService;
            this.logService = logService;
            this.mapper = mapper;
        }

        [FixedDelayRetry(3, "00:05:00")]
        [FunctionName("GoodsReceivalRecipientFunction")]
        [return: ServiceBus("azure-topic-prime-cargo-wms-goods-receival-request", Connection = "serviceBus")]
        public async Task<Message> Run([ServiceBusTrigger("azure-topic-mesage-receiver-from-nav", "azure-goods-receival-recipient-subscription", Connection = "serviceBus")] string mySbMsg, ILogger log)
        {
            try
            {
                log.LogInformation("GoodsReceivalRecipient function recieved the message from the topic");

                var timeLines = new List<TimeLineDTO>();

                // Read file from blob storage
                string fileContent = await this.blobService.DownloadFileByFileNameAsync(mySbMsg);

                // Get goods receival object from the topic message and create it in the storage
                var goodsReceivalDTO = JsonConvert.DeserializeObject<GoodsReceivalDTO>(fileContent);

                var createResponse = await this.goodsReceivalService.CreateGoodsReceivalAsync(goodsReceivalDTO);

                var goodsReceival = createResponse.Entity as GoodsReceivalEntity;

                if (!createResponse.Succeeded && goodsReceival == null)
                {
                    log.LogError(createResponse.Error);
                    return null;
                }

                // Create an erp message and time line
                LogInfo erpInfo = this.mapper.Map<LogInfo>(goodsReceival);
                await this.logService.AddErpMessageAsync(erpInfo, ErpMessageStatus.ReceivedFromErp);
                 
                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.ErpMessageReceived, Status = TimeLineStatus.Information, DateTime = DateTime.UtcNow });

                if (!createResponse.Succeeded)
                {
                    timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.ErrorCreatingGoodsReceival + createResponse.Error, Status = TimeLineStatus.Error, DateTime = DateTime.UtcNow });
                    
                    // Write time lines to database
                    await this.logService.AddTimeLinesAsync(erpInfo, timeLines);

                    log.LogError(createResponse.Error);
                    return null;
                }

                // Validate goods receival
                var validationResults = validationService.Validate(goodsReceival).Concat(goodsReceival.PurchaseLines.SelectMany(x => validationService.Validate(x))).ToList();

                if (validationResults.Count > 0)
                {
                    timeLines.AddRange(validationResults.Select(x => new TimeLineDTO
                    {
                        Description = NavObject.GoodsReceival + TimeLineDescription.ErrorValidation + x.ErrorMessage,
                        Status = TimeLineStatus.Error,
                        DateTime = DateTime.UtcNow
                    }));

                    // Write logs to database
                    await this.logService.AddErpMessageAsync(erpInfo, ErpMessageStatus.Error);
                    await this.logService.AddTimeLinesAsync(erpInfo, timeLines);

                    log.LogError("Validation error");
                    return null;
                }

                // Map the goods receival to the prime cargo request object
                var primeCargoGoodsReceival = this.mapper.Map<PrimeCargoGoodsReceivalRequestDTO>(goodsReceivalDTO);

                primeCargoGoodsReceival.Eta = DateTimeHelper.ConvertErpDateToPrimeCargoDate(goodsReceivalDTO.Eta);
                primeCargoGoodsReceival.ReceivalTypeId = GoodsReceivalType.OrderTypeId;

                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PrepareForServiceBus, Status = TimeLineStatus.Information, DateTime = DateTime.UtcNow });

                // Write time lines to database
                await this.logService.AddTimeLinesAsync(erpInfo, timeLines);

                // Create a topic message
                var messageBody = new RequestMessage<PrimeCargoGoodsReceivalRequestDTO> { ErpInfo = erpInfo, RequestObject = primeCargoGoodsReceival };

                string primeCargoProductJson = JsonConvert.SerializeObject(messageBody);

                var messageProperties = new Dictionary<string, object> { { "type", "create" } };

                return this.serviceBusService.CreateMessage(primeCargoProductJson, messageProperties);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
