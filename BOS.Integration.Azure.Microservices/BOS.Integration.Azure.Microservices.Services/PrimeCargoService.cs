using AutoMapper;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Auth;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Product;
using BOS.Integration.Azure.Microservices.Domain.Enums;
using BOS.Integration.Azure.Microservices.Infrastructure.Configuration;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System;
using System.Globalization;
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

        public async Task<PrimeCargoProductResponseDTO> CreateOrUpdatePrimeCargoProductAsync(PrimeCargoProductRequestDTO primeCargoProduct, ActionType actionType)
        {
            string productUrl = configuration.PrimeCargoSettings.Url + "Product/" + (actionType == ActionType.Create ? "CreateProduct" : "UpdateProduct");
            string authUrl = configuration.PrimeCargoSettings.Url + "Auth";

            var authBody = new PrimeCargoAuthRequestDTO
            {
                OwnerCode = configuration.PrimeCargoSettings.OwnerCode,
                UserName = configuration.PrimeCargoSettings.UserName,
                Password = configuration.PrimeCargoSettings.Password
            };

            var authResponse = await this.httpService.PostAsync<PrimeCargoAuthRequestDTO, PrimeCargoAuthResponseDTO>(authUrl, authBody, configuration.PrimeCargoSettings.Key);

            var content = await this.httpService.PostAsync<PrimeCargoProductRequestDTO, PrimeCargoProductResponseContent>(productUrl, primeCargoProduct, configuration.PrimeCargoSettings.Key, authResponse.Data?.Token);

            var response = this.mapper.Map<PrimeCargoProductResponseDTO>(content);

            response.ErpjobId = primeCargoProduct.ErpjobId;
            response.ResponseDateTime = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);

            return response;
        }
    }
}
