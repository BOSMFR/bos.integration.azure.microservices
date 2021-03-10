using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Auth;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Plytix;
using BOS.Integration.Azure.Microservices.Infrastructure.Configuration;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class PlytixService : IPlytixService
    {
        private List<string> FilterAttributes { get; }

        private readonly IConfigurationManager configuration;
        private readonly IHttpService httpService;
        private readonly IProductAttributeRepository productAttributeRepository;

        public PlytixService(
            IConfigurationManager configuration, 
            IHttpService httpService,
            IProductAttributeRepository productAttributeRepository)
        {
            this.configuration = configuration;
            this.httpService = httpService;
            this.productAttributeRepository = productAttributeRepository;

            this.FilterAttributes = new List<string> { "name", "label", "attributes", "groups", "type_class", "options" };
        }

        public async Task<ActionExecutionResult> SynchronizeProductAttributesAsync()
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                // Get access token
                var authBody = new PlytixAuthRequestDTO
                {
                    Key = configuration.PlytixSettings.Key,
                    Password = configuration.PlytixSettings.Password
                };

                var authResponse = await this.httpService.PostAsync<PlytixAuthRequestDTO, PlytixAuthResponseDTO>(configuration.PlytixSettings.AuthUrl, authBody);

                string token = authResponse?.Data?.FirstOrDefault()?.AccessToken;

                if (string.IsNullOrEmpty(token))
                {
                    actionResult.Entity = authResponse;
                    actionResult.Error = "Could not get access token";

                    return actionResult;
                }

                // Get data from API
                string url = configuration.PlytixSettings.Url + "attributes/product/search";

                var productAttributeSearchRequestBody = new ProductAttributeSearchRequestDTO { Attributes = this.FilterAttributes };

                var content = await this.httpService.PostAsync<ProductAttributeSearchRequestDTO, ProductAttributeSearchResponseDTO>(url, productAttributeSearchRequestBody, null, token);

                if (content?.Data == null || content.StatusCode != Convert.ToInt32(HttpStatusCode.OK).ToString())
                {
                    actionResult.Entity = content;
                    actionResult.Error = "Could not get product attributes";

                    return actionResult;
                }

                // Write data to database
                var productAttributes = content.Data;

                await productAttributeRepository.UpdateRangeAsync(productAttributes);

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
