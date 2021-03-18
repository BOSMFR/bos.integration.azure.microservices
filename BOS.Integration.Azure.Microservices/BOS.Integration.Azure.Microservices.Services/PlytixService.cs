using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Auth;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Plytix;
using BOS.Integration.Azure.Microservices.Domain.Entities.Collection;
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
        private readonly IPlytixRepository plytixRepository;

        public PlytixService(
            IConfigurationManager configuration,
            IHttpService httpService,
            IProductAttributeRepository productAttributeRepository,
            IPlytixRepository plytixRepository)
        {
            this.configuration = configuration;
            this.httpService = httpService;
            this.productAttributeRepository = productAttributeRepository;
            this.plytixRepository = plytixRepository;

            this.FilterAttributes = new List<string> { "name", "label", "attributes", "groups", "type_class", "options" };
        }

        public async Task<ActionExecutionResult> SynchronizeProductAttributesAsync()
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                // Get plytix instances from the storage
                var plytixInstances = await this.plytixRepository.GetActiveInstancesAsync();

                if (plytixInstances?.Count == 0)
                {
                    actionResult.Error = "Could not find any active plytix instances";

                    return actionResult;
                }

                foreach (var plytix in plytixInstances)
                {
                    // Get access token
                    var authBody = new PlytixAuthRequestDTO
                    {
                        Key = plytix.Key,
                        Password = plytix.Paswword
                    };

                    var authResponse = await this.httpService.PostAsync<PlytixAuthRequestDTO, PlytixAuthResponseDTO>(configuration.PlytixSettings.AuthUrl, authBody);

                    string token = authResponse?.Data?.FirstOrDefault()?.AccessToken;

                    if (string.IsNullOrEmpty(token))
                    {
                        actionResult.Entity = authResponse;
                        actionResult.Error = $"Could not get access token for instance {plytix.Name}";

                        return actionResult;
                    }

                    // Get data from API
                    string url = plytix.ServerUrl + "attributes/product/search";

                    var productAttributeSearchRequestBody = new ProductAttributeSearchRequestDTO { Attributes = this.FilterAttributes };

                    var content = await this.httpService.PostAsync<ProductAttributeSearchRequestDTO, ProductAttributeSearchResponseDTO>(url, productAttributeSearchRequestBody, null, token);

                    if (content?.Data == null || content.StatusCode != Convert.ToInt32(HttpStatusCode.OK).ToString())
                    {
                        actionResult.Entity = content;
                        actionResult.Error = $"Could not get product attributes for instance {plytix.Name}";

                        return actionResult;
                    }

                    // Write data to database
                    var productAttributes = content.Data.ToList();

                    productAttributes.ForEach(x => x.PlytixInstanceId = plytix.Id.ToString());

                    await productAttributeRepository.UpdateRangeAsync(productAttributes, plytix.Id.ToString());
                }

                actionResult.Succeeded = true;

                return actionResult;
            }
            catch (Exception ex)
            {
                actionResult.Error = ex.Message;
                return actionResult;
            }
        }

        public async Task<ActionExecutionResult> UpdateCollectionProductAttributeAsync(CollectionEntity collection)
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                // Get plytix instances from the storage
                var plytixInstances = await this.plytixRepository.GetActiveInstancesAsync();

                if (plytixInstances?.Count == 0)
                {
                    actionResult.Error = "Could not find any active plytix instances";

                    return actionResult;
                }

                foreach (var plytix in plytixInstances)
                {
                    // Get access token
                    var authBody = new PlytixAuthRequestDTO
                    {
                        Key = plytix.Key,
                        Password = plytix.Paswword
                    };

                    var authResponse = await this.httpService.PostAsync<PlytixAuthRequestDTO, PlytixAuthResponseDTO>(configuration.PlytixSettings.AuthUrl, authBody);

                    string token = authResponse?.Data?.FirstOrDefault()?.AccessToken;

                    if (string.IsNullOrEmpty(token))
                    {
                        actionResult.Entity = authResponse;
                        actionResult.Error = $"Could not get access token for instance {plytix.Name}";

                        return actionResult;
                    }

                    // Get collection product attribute from the storage
                    var productAttribute = await productAttributeRepository.GetByLabelAsync(collection.Category, plytix.Id.ToString());

                    if (productAttribute == null)
                    {
                        actionResult.Entity = productAttribute;
                        actionResult.Error = $"Could not get product attribute with label: {collection.Category} for instance {plytix.Name}";

                        return actionResult;
                    }

                    productAttribute.Options = collection.Details.Where(x => x.ShowExternal).Select(x => x.Id).ToList();

                    // Update collection product attribute in plytix
                    string updateUrl = plytix.ServerUrl + "attributes/product/" + productAttribute.Id;

                    var updateRequestBody = new ProductAttributeUpdateRequestDTO()
                    {
                        Options = productAttribute.Options
                    };

                    var updateResponse = await this.httpService.PatchAsync<ProductAttributeUpdateRequestDTO, ProductAttributeUpdateResponseDTO>(updateUrl, updateRequestBody, token);

                    if (updateResponse.StatusCode != Convert.ToInt32(HttpStatusCode.OK).ToString())
                    {
                        actionResult.Error = $"Could not update product attribute for instance {plytix.Name}";

                        return actionResult;
                    }

                    // Update product attribute in the storage
                    await productAttributeRepository.UpdateAsync(productAttribute, productAttribute.PlytixInstanceId);
                }

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
