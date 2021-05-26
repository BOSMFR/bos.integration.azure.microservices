using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Auth;
using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PrimeCargo;
using BOS.Integration.Azure.Microservices.Domain.Enums;
using BOS.Integration.Azure.Microservices.Infrastructure.Configuration;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using BOS.Integration.Azure.Microservices.Services.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class PrimeCargoService : IPrimeCargoService
    {
        private readonly IConfigurationManager configuration;
        private readonly IHttpService httpService;
        private readonly ILogService logService;

        public PrimeCargoService(
            IConfigurationManager configuration,
            IHttpService httpService,
            ILogService logService)
        {
            this.configuration = configuration;
            this.httpService = httpService;
            this.logService = logService;
        }

        public async Task<PrimeCargoResponseContent<V>> CreateOrUpdatePrimeCargoObjectAsync<T, V>(RequestMessage<T> messageObject, ILogger log, string entityName, ActionType actionType)
        {
            var erpMessageStatuses = new List<string>();
            var timeLines = new List<TimeLineDTO>();

            erpMessageStatuses.Add(actionType == ActionType.Create ? ErpMessageStatus.CreateMessage : ErpMessageStatus.UpdateMessage);

            timeLines.Add(new TimeLineDTO
            {
                Description = entityName + (actionType == ActionType.Create ? TimeLineDescription.PrimeCargoCreateMessageSentServiceBus : TimeLineDescription.PrimeCargoUpdateMessageSentServiceBus),
                Status = TimeLineStatus.Information,
                DateTime = DateTime.UtcNow
            });

            // Use prime cargo API to process the object
            string createUrl = configuration.PrimeCargoSettings.Url + entityName + "/" + (actionType == ActionType.Create ? "Create" : "Update") + entityName;

            var response = await this.CallPrimeCargoPostEndpointAsync<T, V>(createUrl, messageObject.RequestObject);

            if (response.Succeeded)
            {
                erpMessageStatuses.Add(ErpMessageStatus.DeliveredSuccessfully);
                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.DeliveredSuccessfullyToPrimeCargo, Status = TimeLineStatus.Successfully, DateTime = DateTime.UtcNow });
            }
            else
            {
                string customError = actionType == ActionType.Create ? "Could not create a new object via prime cargo API" : "Could not update the object via prime cargo API";

                string errorMessage = string.IsNullOrEmpty(response.Error) ? customError : response.Error;
                log.LogError(errorMessage);

                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PrimeCargoRequestError + errorMessage, Status = TimeLineStatus.Error, DateTime = DateTime.UtcNow });

                // Write erp messages and time lines to database
                await this.logService.AddErpMessagesAsync(messageObject.ErpInfo, erpMessageStatuses);
                await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

                return null;
            }

            var responseContent = response.Entity as PrimeCargoResponseContent<V>;

            if (responseContent?.StatusCode == Convert.ToInt32(HttpStatusCode.RequestTimeout).ToString())
            {
                erpMessageStatuses.Add(actionType == ActionType.Create ? ErpMessageStatus.CreateTimeout : ErpMessageStatus.UpdateTimeout);
                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PrimeCargoRequestTimeOut, Status = TimeLineStatus.Error, DateTime = DateTime.UtcNow });

                // Write erp messages and time lines to database
                await this.logService.AddErpMessagesAsync(messageObject.ErpInfo, erpMessageStatuses);
                await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

                throw new Exception("Request time out");
            }

            // Write erp messages and time lines to database
            await this.logService.AddErpMessagesAsync(messageObject.ErpInfo, erpMessageStatuses);
            await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

            return responseContent;            
        }

        public async Task<ActionExecutionResult> GetGoodsReceivalsByLastUpdateAsync(DateTime lastUpdate)
        {
            string url = configuration.PrimeCargoSettings.Url + "GoodsReceival/GetGoodsReceivalsByReceivalLineUpdate?Lastupdate=" + lastUpdate.ToString(DateHelper.PrimeCargoDateFormat);

            return await this.CallPrimeCargoGetEndpointAsync<List<PrimeCargoGoodsReceivalResponseDTO>>(url);
        }

        public async Task<ActionExecutionResult> CallPrimeCargoPostEndpointAsync<T, V>(string url, T primeCargoRequestObject)
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

        public async Task<ActionExecutionResult> CallPrimeCargoGetEndpointAsync<T>(string url)
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
