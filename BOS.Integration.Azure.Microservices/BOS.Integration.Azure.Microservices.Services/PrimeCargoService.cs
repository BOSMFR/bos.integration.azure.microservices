using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Auth;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Domain.Enums;
using BOS.Integration.Azure.Microservices.Infrastructure.Configuration;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class PrimeCargoService : IPrimeCargoService
    {
        private readonly IConfigurationManager configuration;
        private readonly IHttpService httpService;
        private readonly IMapper mapper;

        public PrimeCargoService(IConfigurationManager configuration, IHttpService httpService, IMapper mapper)
        {
            this.configuration = configuration;
            this.httpService = httpService;
            this.mapper = mapper;
        }

        public async Task<ActionExecutionResult> CreateOrUpdatePrimeCargoProductAsync(PrimeCargoProductRequestDTO primeCargoProduct, ActionType actionType)
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
