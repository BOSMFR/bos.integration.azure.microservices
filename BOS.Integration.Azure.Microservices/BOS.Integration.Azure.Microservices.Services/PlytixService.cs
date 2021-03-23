using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Auth;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Plytix;
using BOS.Integration.Azure.Microservices.Domain.Entities.Plytix;
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
        private List<string> ProductAttibuteFilterAttributes { get; }
        private List<string> AssetCategoryFilterAttributes { get; }

        private readonly IConfigurationManager configuration;
        private readonly IHttpService httpService;
        private readonly IProductAttributeRepository productAttributeRepository;
        private readonly IAssetCategoryRepository assetCategoryRepository;
        private readonly IPlytixRepository plytixRepository;

        public PlytixService(
            IConfigurationManager configuration,
            IHttpService httpService,
            IProductAttributeRepository productAttributeRepository,
            IAssetCategoryRepository assetCategoryRepository,
            IPlytixRepository plytixRepository)
        {
            this.configuration = configuration;
            this.httpService = httpService;
            this.productAttributeRepository = productAttributeRepository;
            this.assetCategoryRepository = assetCategoryRepository;
            this.plytixRepository = plytixRepository;

            this.ProductAttibuteFilterAttributes = new List<string> { "name", "label", "attributes", "groups", "type_class", "options" };
            this.AssetCategoryFilterAttributes = new List<string> { "name", "n_children" };
        }

        public async Task<ActionExecutionResult> SynchronizeProductAttributesAsync()
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                var response = await GetPlytixInstancesWithTokensAsync();

                if (!response.Succeeded)
                {
                    actionResult.Error = response.Error;
                    return actionResult;
                }

                var plytixData = response.Entity as List<PlytixInstanceDTO>;

                foreach (var data in plytixData)
                {
                    // Get data from API
                    string url = data.Plytix.ServerUrl + "attributes/product/search";

                    var productAttributeSearchRequestBody = new PlytixSearchRequestDTO { Attributes = this.ProductAttibuteFilterAttributes };

                    var content = await this.httpService.PostAsync<PlytixSearchRequestDTO, PlytixSearchResponseDTO<ProductAttribute>>(url, productAttributeSearchRequestBody, null, data.Token);

                    if (content?.Data == null || content.StatusCode != Convert.ToInt32(HttpStatusCode.OK).ToString())
                    {
                        actionResult.Entity = content;
                        actionResult.Error = $"Could not get product attributes for instance {data.Plytix.Name}";

                        return actionResult;
                    }

                    // Write data to database
                    var productAttributes = content.Data.ToList();

                    productAttributes.ForEach(x => x.PlytixInstanceId = data.Plytix.Id.ToString());

                    await productAttributeRepository.UpdateRangeAsync(productAttributes, data.Plytix.Id.ToString());
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

        public async Task<ActionExecutionResult> SynchronizeAssetCategoriesAsync()
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                var response = await GetPlytixInstancesWithTokensAsync();

                if (!response.Succeeded)
                {
                    actionResult.Error = response.Error;
                    return actionResult;
                }

                var plytixData = response.Entity as List<PlytixInstanceDTO>;

                foreach (var data in plytixData)
                {
                    // Get data from API
                    string url = data.Plytix.ServerUrl + "categories/file/search";

                    var productAttributeSearchRequestBody = new PlytixSearchRequestDTO { Attributes = this.AssetCategoryFilterAttributes };

                    var content = await this.httpService.PostAsync<PlytixSearchRequestDTO, PlytixSearchResponseDTO<AssetCategory>>(url, productAttributeSearchRequestBody, null, data.Token);

                    if (content?.Data == null || content.StatusCode != Convert.ToInt32(HttpStatusCode.OK).ToString())
                    {
                        actionResult.Entity = content;
                        actionResult.Error = $"Could not get asset categories for instance {data.Plytix.Name}";

                        return actionResult;
                    }

                    // Write data to database
                    var assetCategories = content.Data.ToList();

                    assetCategories.ForEach(x => x.PlytixInstanceId = data.Plytix.Id.ToString());

                    await assetCategoryRepository.UpdateRangeAsync(assetCategories, data.Plytix.Id.ToString());
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

        public async Task<ActionExecutionResult> UpdateProductAttributeOptionsAsync(string attributeLabel, IEnumerable<string> newOptions)
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                var response = await GetPlytixInstancesWithTokensAsync();

                if (!response.Succeeded)
                {
                    actionResult.Error = response.Error;
                    return actionResult;
                }

                var plytixData = response.Entity as List<PlytixInstanceDTO>;

                foreach (var data in plytixData)
                {
                    // Get collection product attribute from the storage
                    var productAttribute = await productAttributeRepository.GetByLabelAsync(attributeLabel, data.Plytix.Id.ToString());

                    if (productAttribute == null)
                    {
                        actionResult.Entity = productAttribute;
                        actionResult.Error = $"Could not get product attribute with label: {attributeLabel} for instance {data.Plytix.Name}";

                        return actionResult;
                    }

                    productAttribute.Options = newOptions.ToList();

                    // Update collection product attribute in plytix
                    string updateUrl = data.Plytix.ServerUrl + "attributes/product/" + productAttribute.Id;

                    var updateRequestBody = new ProductAttributeUpdateRequestDTO()
                    {
                        Options = productAttribute.Options
                    };

                    var updateResponse = await this.httpService.PatchAsync<ProductAttributeUpdateRequestDTO, ProductAttributeUpdateResponseDTO>(updateUrl, updateRequestBody, data.Token);

                    if (updateResponse.StatusCode != Convert.ToInt32(HttpStatusCode.OK).ToString())
                    {
                        actionResult.Error = $"Could not update product attribute with label: {attributeLabel} for instance {data.Plytix.Name}";

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

        private async Task<ActionExecutionResult> GetPlytixInstancesWithTokensAsync()
        {
            var actionResult = new ActionExecutionResult();
            var plytixData = new List<PlytixInstanceDTO>();

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

                    plytixData.Add(new PlytixInstanceDTO { Plytix = plytix, Token = token });
                }

                actionResult.Entity = plytixData;
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
