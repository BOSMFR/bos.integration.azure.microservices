using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Auth;
using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PrimeCargo;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Domain.Enums;
using BOS.Integration.Azure.Microservices.Infrastructure.Configuration;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class PrimeCargoService : IPrimeCargoService
    {
        private readonly IValidationService validationService;
        private readonly IServiceBusService serviceBusService;
        private readonly IConfigurationManager configuration;
        private readonly IHttpService httpService;
        private readonly ILogService logService;
        private readonly IMapper mapper;

        public PrimeCargoService(
            IValidationService validationService,
            IServiceBusService serviceBusService,
            IConfigurationManager configuration, 
            IHttpService httpService,
            ILogService logService,
            IMapper mapper)
        {
            this.validationService = validationService;
            this.serviceBusService = serviceBusService;
            this.configuration = configuration;
            this.httpService = httpService;
            this.logService = logService;
            this.mapper = mapper;
        }

        public async Task<Message> CreateOrUpdatePrimeCargoGoodsReceivalAsync(string mySbMsg, ILogger log, ActionType actionType)
        {
            var erpMessageStatuses = new List<string>();
            var timeLines = new List<TimeLineDTO>();

            // Deserialize prime cargo goods receival from the message
            var messageObject = JsonConvert.DeserializeObject<PrimeCargoRequestMessage<PrimeCargoGoodsReceivalRequestDTO>>(mySbMsg);

            erpMessageStatuses.Add(actionType == ActionType.Create ? ErpMessageStatus.CreateMessage : ErpMessageStatus.UpdateMessage);

            timeLines.Add(new TimeLineDTO
            {
                Description = actionType == ActionType.Create ? TimeLineDescription.GoodsReceivalCreateMessageSentServiceBus : TimeLineDescription.GoodsReceivalUpdateMessageSentServiceBus,
                Status = TimeLineStatus.Information,
                DateTime = DateTime.Now
            });

            // Use prime cargo API to process the object
            string goodsReceivalUrl = configuration.PrimeCargoSettings.Url + "GoodsReceival/CreateGoodsReceival";

            var response = await this.CallPrimeCargoEndpointAsync<PrimeCargoGoodsReceivalRequestDTO, PrimeCargoGoodsReceivalResponseDTO>(goodsReceivalUrl, messageObject.PrimeCargoRequestObject);

            if (response.Succeeded)
            {
                erpMessageStatuses.Add(ErpMessageStatus.DeliveredSuccessfully);
                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.DeliveredSuccessfullyToPrimeCargo, Status = TimeLineStatus.Successfully, DateTime = DateTime.Now });
            }
            else
            {
                string customError = actionType == ActionType.Create ? "Could not create a new object via prime cargo API" : "Could not update the object via prime cargo API";

                string errorMessage = string.IsNullOrEmpty(response.Error) ? customError : response.Error;
                log.LogError(errorMessage);

                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PrimeCargoRequestError + errorMessage, Status = TimeLineStatus.Error, DateTime = DateTime.Now });
            }

            var responseContent = response.Entity as PrimeCargoResponseContent<PrimeCargoGoodsReceivalResponseDTO>;

            if (responseContent?.StatusCode == Convert.ToInt32(HttpStatusCode.RequestTimeout).ToString())
            {
                erpMessageStatuses.Add(actionType == ActionType.Create ? ErpMessageStatus.CreateTimeout : ErpMessageStatus.UpdateTimeout);
                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PrimeCargoRequestTimeOut, Status = TimeLineStatus.Error, DateTime = DateTime.Now });

                throw new Exception("Request time out");
            }

            // Write erp messages and time lines to database
            await this.logService.AddErpMessagesAsync(messageObject.ErpInfo, erpMessageStatuses);
            await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

            // Create a topic message
            var messageBody = new PrimeCargoResponseMessage<PrimeCargoGoodsReceivalResponseDTO> { ErpInfo = messageObject.ErpInfo, PrimeCargoResponseObject = responseContent?.Data };

            string primeCargoProductResponseJson = JsonConvert.SerializeObject(messageBody);

            return this.serviceBusService.CreateMessage(primeCargoProductResponseJson);
        }

        public async Task<Message> CreateOrUpdatePrimeCargoProductAsync(string mySbMsg, ILogger log, ActionType actionType)
        {
            var erpMessageStatuses = new List<string>();
            var timeLines = new List<TimeLineDTO>();

            // Deserialize prime cargo product from the message and validate it
            var messageObject = JsonConvert.DeserializeObject<PrimeCargoRequestMessage<PrimeCargoProductRequestDTO>>(mySbMsg);

            erpMessageStatuses.Add(actionType == ActionType.Create ? ErpMessageStatus.CreateMessage : ErpMessageStatus.UpdateMessage);

            if (!validationService.Validate(messageObject.PrimeCargoRequestObject))
            {
                erpMessageStatuses.Add(ErpMessageStatus.Error);

                // Write erp messages and a line to database
                await this.logService.AddErpMessagesAsync(messageObject.ErpInfo, erpMessageStatuses);
                await this.logService.AddTimeLineAsync(messageObject.ErpInfo, TimeLineDescription.DataValidationFailed, TimeLineStatus.Error);

                log.LogError("Prime Cargo object validation error occured");
                return null;
            }

            timeLines.Add(new TimeLineDTO 
            {
                Description = actionType == ActionType.Create ? TimeLineDescription.ProductCreateMessageSentServiceBus : TimeLineDescription.ProductUpdateMessageSentServiceBus,
                Status = TimeLineStatus.Information, DateTime = DateTime.Now 
            });

            // Use prime cargo API to process the object
            string productUrl = configuration.PrimeCargoSettings.Url + "Product/" + (actionType == ActionType.Create ? "CreateProduct" : "UpdateProduct");

            var response = await this.CallPrimeCargoEndpointAsync<PrimeCargoProductRequestDTO, PrimeCargoProductResponseData>(productUrl, messageObject.PrimeCargoRequestObject);

            if (response.Succeeded)
            {
                erpMessageStatuses.Add(ErpMessageStatus.DeliveredSuccessfully);
                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.DeliveredSuccessfullyToPrimeCargo, Status = TimeLineStatus.Successfully, DateTime = DateTime.Now });
            }
            else
            {
                string customError = actionType == ActionType.Create ? "Could not create a new object via prime cargo API" : "Could not update the object via prime cargo API";

                string errorMessage = string.IsNullOrEmpty(response.Error) ? customError : response.Error;
                log.LogError(errorMessage);

                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PrimeCargoRequestError + errorMessage, Status = TimeLineStatus.Error, DateTime = DateTime.Now });
            }

            var responseContent = response.Entity as PrimeCargoResponseContent<PrimeCargoProductResponseData>;            

            var primeCargoResponse = this.mapper.Map<PrimeCargoProductResponseDTO>(responseContent);
            primeCargoResponse.ErpjobId = messageObject.PrimeCargoRequestObject.ErpjobId;

            if (primeCargoResponse?.ResponseCode == Convert.ToInt32(HttpStatusCode.RequestTimeout).ToString())
            {
                erpMessageStatuses.Add(actionType == ActionType.Create ? ErpMessageStatus.CreateTimeout : ErpMessageStatus.UpdateTimeout);
                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PrimeCargoRequestTimeOut, Status = TimeLineStatus.Error, DateTime = DateTime.Now });

                throw new Exception("Request time out");
            }

            if (primeCargoResponse == null)
            {
                primeCargoResponse = new PrimeCargoProductResponseDTO
                {
                    EnaNo = messageObject.PrimeCargoRequestObject.Barcode,
                    Success = response.Succeeded
                };
            }

            // Write erp messages and time lines to database
            await this.logService.AddErpMessagesAsync(messageObject.ErpInfo, erpMessageStatuses);
            await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

            // Create a topic message
            var messageBody = new PrimeCargoResponseMessage<PrimeCargoProductResponseDTO> { ErpInfo = messageObject.ErpInfo, PrimeCargoResponseObject = primeCargoResponse };

            string primeCargoProductResponseJson = JsonConvert.SerializeObject(messageBody);

            return this.serviceBusService.CreateMessage(primeCargoProductResponseJson);
        }

        private async Task<ActionExecutionResult> CallPrimeCargoEndpointAsync<T, V>(string url, T primeCargoRequestObject)
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                string authUrl = configuration.PrimeCargoSettings.Url + "Auth";

                var authBody = new PrimeCargoAuthRequestDTO
                {
                    OwnerCode = configuration.PrimeCargoSettings.OwnerCode,
                    UserName = configuration.PrimeCargoSettings.UserName,
                    Password = configuration.PrimeCargoSettings.Password
                };

                var authResponse = await this.httpService.GetAsync<PrimeCargoAuthResponseDTO>(authUrl, configuration.PrimeCargoSettings.Key, authBody: authBody);

                var content = await this.httpService.PostAsync<T, PrimeCargoResponseContent<V>>(url, primeCargoRequestObject, configuration.PrimeCargoSettings.Key, authResponse?.Data?.Token);

                string errorMessage = content.ProcessingDetails?.FirstOrDefault()?.Message ?? content.Error;

                if (!content.Success && !string.IsNullOrEmpty(errorMessage))
                {
                    actionResult.Error = errorMessage;
                    return actionResult;
                }

                actionResult.Entity = content;
                actionResult.Succeeded = true;

                return actionResult;
            }
            catch (Exception ex)
            {
                actionResult.Error = ex.Message;
                return actionResult;
            }
        }
    }
}
