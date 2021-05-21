using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Auth;
using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PrimeCargo;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Domain.Enums;
using BOS.Integration.Azure.Microservices.Infrastructure.Configuration;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using BOS.Integration.Azure.Microservices.Services.Helpers;
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
        private readonly IServiceBusService serviceBusService;
        private readonly IConfigurationManager configuration;
        private readonly IHttpService httpService;
        private readonly ILogService logService;
        private readonly IMapper mapper;

        public PrimeCargoService(
            IServiceBusService serviceBusService,
            IConfigurationManager configuration, 
            IHttpService httpService,
            ILogService logService,
            IMapper mapper)
        {
            this.serviceBusService = serviceBusService;
            this.configuration = configuration;
            this.httpService = httpService;
            this.logService = logService;
            this.mapper = mapper;
        }


        public async Task<Message> CreateOrUpdatePrimeCargoProductAsync(string mySbMsg, ILogger log, ActionType actionType)
        {
            var erpMessageStatuses = new List<string>();
            var timeLines = new List<TimeLineDTO>();

            // Deserialize prime cargo product from the message
            var messageObject = JsonConvert.DeserializeObject<RequestMessage<PrimeCargoProductRequestDTO>>(mySbMsg);

            erpMessageStatuses.Add(actionType == ActionType.Create ? ErpMessageStatus.CreateMessage : ErpMessageStatus.UpdateMessage);            

            timeLines.Add(new TimeLineDTO 
            {
                Description = actionType == ActionType.Create ? TimeLineDescription.ProductCreateMessageSentServiceBus : TimeLineDescription.ProductUpdateMessageSentServiceBus,
                Status = TimeLineStatus.Information, DateTime = DateTime.Now 
            });

            // Use prime cargo API to process the object
            string productUrl = configuration.PrimeCargoSettings.Url + "Product/" + (actionType == ActionType.Create ? "CreateProduct" : "UpdateProduct");

            var response = await this.CallPrimeCargoPostEndpointAsync<PrimeCargoProductRequestDTO, PrimeCargoProductResponseData>(productUrl, messageObject.RequestObject);

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

            if (primeCargoResponse?.ResponseCode == Convert.ToInt32(HttpStatusCode.RequestTimeout).ToString())
            {
                erpMessageStatuses.Add(actionType == ActionType.Create ? ErpMessageStatus.CreateTimeout : ErpMessageStatus.UpdateTimeout);
                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PrimeCargoRequestTimeOut, Status = TimeLineStatus.Error, DateTime = DateTime.Now });

                // Write erp messages and time lines to database
                await this.logService.AddErpMessagesAsync(messageObject.ErpInfo, erpMessageStatuses);
                await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

                throw new Exception("Request time out");
            }

            if (primeCargoResponse == null)
            {
                primeCargoResponse = new PrimeCargoProductResponseDTO
                {
                    EnaNo = messageObject.RequestObject.Barcode,
                    Success = response.Succeeded
                };
            }

            primeCargoResponse.ErpjobId = messageObject.RequestObject.ErpjobId;

            // Write erp messages and time lines to database
            await this.logService.AddErpMessagesAsync(messageObject.ErpInfo, erpMessageStatuses);
            await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

            // Create a topic message
            var messageBody = new ResponseMessage<PrimeCargoProductResponseDTO> { ErpInfo = messageObject.ErpInfo, ResponseObject = primeCargoResponse };

            string primeCargoProductResponseJson = JsonConvert.SerializeObject(messageBody);

            return this.serviceBusService.CreateMessage(primeCargoProductResponseJson);
        }

        public async Task<Message> CreateOrUpdatePrimeCargoGoodsReceivalAsync(string mySbMsg, ILogger log, ActionType actionType)
        {
            var erpMessageStatuses = new List<string>();
            var timeLines = new List<TimeLineDTO>();

            // Deserialize prime cargo goods receival from the message
            var messageObject = JsonConvert.DeserializeObject<RequestMessage<PrimeCargoGoodsReceivalRequestDTO>>(mySbMsg);

            erpMessageStatuses.Add(actionType == ActionType.Create ? ErpMessageStatus.CreateMessage : ErpMessageStatus.UpdateMessage);

            timeLines.Add(new TimeLineDTO
            {
                Description = actionType == ActionType.Create ? TimeLineDescription.GoodsReceivalCreateMessageSentServiceBus : TimeLineDescription.GoodsReceivalUpdateMessageSentServiceBus,
                Status = TimeLineStatus.Information,
                DateTime = DateTime.Now
            });

            // Use prime cargo API to process the object
            string goodsReceivalUrl = configuration.PrimeCargoSettings.Url + "GoodsReceival/CreateGoodsReceival";

            var response = await this.CallPrimeCargoPostEndpointAsync<PrimeCargoGoodsReceivalRequestDTO, PrimeCargoGoodsReceivalResponseDTO>(goodsReceivalUrl, messageObject.RequestObject);

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

                // Write erp messages and time lines to database
                await this.logService.AddErpMessagesAsync(messageObject.ErpInfo, erpMessageStatuses);
                await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

                return null;
            }

            var responseContent = response.Entity as PrimeCargoResponseContent<PrimeCargoGoodsReceivalResponseDTO>;

            if (responseContent?.StatusCode == Convert.ToInt32(HttpStatusCode.RequestTimeout).ToString())
            {
                erpMessageStatuses.Add(actionType == ActionType.Create ? ErpMessageStatus.CreateTimeout : ErpMessageStatus.UpdateTimeout);
                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PrimeCargoRequestTimeOut, Status = TimeLineStatus.Error, DateTime = DateTime.Now });

                // Write erp messages and time lines to database
                await this.logService.AddErpMessagesAsync(messageObject.ErpInfo, erpMessageStatuses);
                await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

                throw new Exception("Request time out");
            }

            // Write erp messages and time lines to database
            await this.logService.AddErpMessagesAsync(messageObject.ErpInfo, erpMessageStatuses);
            await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

            // Create a topic message
            var messageBody = new ResponseMessage<PrimeCargoGoodsReceivalResponseDTO> { ErpInfo = messageObject.ErpInfo, ResponseObject = responseContent?.Data };

            string primeCargoProductResponseJson = JsonConvert.SerializeObject(messageBody);

            return this.serviceBusService.CreateMessage(primeCargoProductResponseJson);
        }

        public async Task<Message> CreateOrUpdatePrimeCargoPickOrderAsync(string mySbMsg, ILogger log, ActionType actionType)
        {
            return null;
        }

        public async Task<ActionExecutionResult> GetPrimeCargoGoodsReceivalByIdAsync(string id)
        {
            string url = configuration.PrimeCargoSettings.Url + "GoodsReceival/GetGoodsReceival?id=" + id;

            return await this.CallPrimeCargoGetEndpointAsync<PrimeCargoGoodsReceivalResponseDTO>(url);
        }

        public async Task<ActionExecutionResult> GetGoodsReceivalsByLastUpdateAsync(DateTime lastUpdate)
        {
            string url = configuration.PrimeCargoSettings.Url + "GoodsReceival/GetGoodsReceivalsByReceivalLineUpdate?Lastupdate=" + lastUpdate.ToString(DateHelper.PrimeCargoDateFormat);

            return await this.CallPrimeCargoGetEndpointAsync<List<PrimeCargoGoodsReceivalResponseDTO>>(url);
        }

        private async Task<ActionExecutionResult> CallPrimeCargoPostEndpointAsync<T, V>(string url, T primeCargoRequestObject)
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

                string errorMessage = content.ProcessingDetails?.FirstOrDefault()?.Message;

                if (!content.Success)
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

        private async Task<ActionExecutionResult> CallPrimeCargoGetEndpointAsync<T>(string url)
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

                var content = await this.httpService.GetAsync<PrimeCargoResponseContent<T>>(url, configuration.PrimeCargoSettings.Key, authResponse?.Data?.Token);

                string errorMessage = content.ProcessingDetails?.FirstOrDefault()?.Message;

                if (!content.Success)
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
