using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Auth;
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

        public async Task<Message> CreateOrUpdatePrimeCargoProductAsync(string mySbMsg, ILogger log, ActionType actionType)
        {
            var erpMessageStatuses = new List<string>();
            var timeLines = new List<TimeLineDTO>();

            // Deserialize prime cargo product from the message and validate it
            var messageObject = JsonConvert.DeserializeObject<PrimeCargoProductRequestMessage>(mySbMsg);

            erpMessageStatuses.Add(actionType == ActionType.Create ? ErpMessageStatus.CreateMessage : ErpMessageStatus.UpdateMessage);

            if (!validationService.Validate(messageObject.PrimeCargoProduct))
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
            var response = await this.CallPrimeCargoCreateOrUpdateProductEndpointAsync(messageObject.PrimeCargoProduct, actionType);

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

            var primeCargoResponse = response.Entity as PrimeCargoProductResponseDTO;

            if (primeCargoResponse?.ResponseCode == Convert.ToInt32(HttpStatusCode.RequestTimeout).ToString())
            {
                erpMessageStatuses.Add(actionType == ActionType.Create ? ErpMessageStatus.CreateTimeout : ErpMessageStatus.UpdateTimeout);
                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PrimeCargoRequestTimeOut, Status = TimeLineStatus.Error, DateTime = DateTime.Now });
            }

            if (primeCargoResponse == null)
            {
                primeCargoResponse = new PrimeCargoProductResponseDTO
                {
                    EnaNo = messageObject.PrimeCargoProduct.Barcode,
                    Success = response.Succeeded
                };
            }

            // Write erp messages and time lines to database
            await this.logService.AddErpMessagesAsync(messageObject.ErpInfo, erpMessageStatuses);
            await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

            // Create a topic message
            var messageBody = new PrimeCargoProductResponseMessage { ErpInfo = messageObject.ErpInfo, PrimeCargoProduct = primeCargoResponse };

            string primeCargoProductResponseJson = JsonConvert.SerializeObject(messageBody);

            return this.serviceBusService.CreateMessage(primeCargoProductResponseJson);
        }

        private async Task<ActionExecutionResult> CallPrimeCargoCreateOrUpdateProductEndpointAsync(PrimeCargoProductRequestDTO primeCargoProduct, ActionType actionType)
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                string productUrl = configuration.PrimeCargoSettings.Url + "Product/" + (actionType == ActionType.Create ? "CreateProduct" : "UpdateProduct");
                string authUrl = configuration.PrimeCargoSettings.Url + "Auth";

                var authBody = new PrimeCargoAuthRequestDTO
                {
                    OwnerCode = configuration.PrimeCargoSettings.OwnerCode,
                    UserName = configuration.PrimeCargoSettings.UserName,
                    Password = configuration.PrimeCargoSettings.Password
                };

                var authResponse = await this.httpService.GetAsync<PrimeCargoAuthResponseDTO>(authUrl, configuration.PrimeCargoSettings.Key, authBody: authBody);

                var content = await this.httpService.PostAsync<PrimeCargoProductRequestDTO, PrimeCargoProductResponseContent>(productUrl, primeCargoProduct, configuration.PrimeCargoSettings.Key, authResponse?.Data?.Token);

                string errorMessage = content.ProcessingDetails?.FirstOrDefault()?.Message;

                if (!content.Success && !string.IsNullOrEmpty(errorMessage))
                {
                    actionResult.Error = errorMessage;
                    return actionResult;
                }

                var responseObject = this.mapper.Map<PrimeCargoProductResponseDTO>(content);

                responseObject.ErpjobId = primeCargoProduct.ErpjobId;

                actionResult.Entity = responseObject;
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
