using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Auth;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Plytix;
using BOS.Integration.Azure.Microservices.Domain.Entities;
using BOS.Integration.Azure.Microservices.Domain.Entities.Plytix;
using BOS.Integration.Azure.Microservices.Infrastructure.Configuration;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class PlytixService : IPlytixService
    {
        private const int DefaultPageSize = 100;
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
            this.AssetCategoryFilterAttributes = new List<string> { "name", "n_children", "path", "parents_ids", "order" };
        }

        public async Task<PlytixSyncResultDTO> SynchronizePlytixOptionsAsync(IEnumerable<string> collectionOptions, IEnumerable<string> deliveryPeriodOptions)
        {
            var actionResult = new PlytixSyncResultDTO();

            try
            {
                var response = await GetPlytixInstancesWithTokensAsync();

                if (!response.Succeeded)
                {
                    actionResult.GeneralResult.Error = response.Error;
                    return actionResult;
                }

                var plytixData = response.Entity as List<PlytixInstanceDTO>;

                // Sync Collection options
                actionResult.UpdateCollectionResult = await this.UpdatePlytixOptionsAsync(NavObjectCategory.Collection, collectionOptions, plytixData);

                // Sync Delivery period options
                actionResult.UpdateDeliveryPeriodResult = await this.UpdatePlytixOptionsAsync(NavObjectCategory.DeliveryPeriod, deliveryPeriodOptions, plytixData);

                actionResult.GeneralResult = new ActionExecutionResult() { Succeeded = actionResult.UpdateCollectionResult.Succeeded && actionResult.UpdateDeliveryPeriodResult.Succeeded };

                return actionResult;
            }
            catch (Exception ex)
            {
                actionResult.GeneralResult.Error = ex.Message;
                return actionResult;
            }
        }

        public async Task<ActionExecutionResult> UpdatePlytixOptionsAsync(string label, IEnumerable<string> options, List<PlytixInstanceDTO> plytixInstances = null)
        {
            var actionResult = new ActionExecutionResult();
            var timeLines = new List<TimeLineDTO>();

            try
            {
                if (plytixInstances == null)
                {
                    var response = await GetPlytixInstancesWithTokensAsync();

                    if (!response.Succeeded)
                    {
                        actionResult.Error = response.Error;
                        return actionResult;
                    }

                    plytixInstances = response.Entity as List<PlytixInstanceDTO>;
                }                

                foreach (var plytixInstance in plytixInstances)
                {
                    // Synchronize product attributes in the storage with Plytix
                    var synchronizeProductAttributesResponse = await SynchronizeProductAttributesAsync(plytixInstance);

                    if (!synchronizeProductAttributesResponse.Succeeded)
                    {
                        actionResult.Error = synchronizeProductAttributesResponse.Error;
                        return actionResult;
                    }

                    // Update product attributes in the storage and Plytix
                    var updateProductAttributeResponse = await UpdateProductAttributeAsync(plytixInstance, label, options);

                    if (updateProductAttributeResponse.Succeeded)
                    {
                        string description = label == NavObjectCategory.Collection ? TimeLineDescription.CollectionPaUpdatedSuccessfully : TimeLineDescription.DeliveryPeriodPaUpdatedSuccessfully;
                        timeLines.Add(new TimeLineDTO { Description = description, Status = TimeLineStatus.Successfully, DateTime = DateTime.Now });
                    }
                    else
                    {
                        string description = label == NavObjectCategory.Collection ? TimeLineDescription.CollectionPaUpdateError : TimeLineDescription.DeliveryPeriodPaUpdateError;
                        description += updateProductAttributeResponse.Error;
                        timeLines.Add(new TimeLineDTO { Description = description, Status = TimeLineStatus.Error, DateTime = DateTime.Now });
                    }

                    // Synchronize asset categories in the storage with Plytix
                    var synchronizeAssetCategoriesResponse = await this.SynchronizeAssetCategoriesAsync(plytixInstance);

                    if (!synchronizeAssetCategoriesResponse.Succeeded)
                    {
                        actionResult.Error = synchronizeAssetCategoriesResponse.Error;
                        actionResult.Entity = timeLines;
                        return actionResult;
                    }

                    // Get not existing in Plytix asset categories
                    var assetCategoriesInPlytix = synchronizeAssetCategoriesResponse.Entity as List<AssetCategory>;

                    var newOptions = assetCategoriesInPlytix != null ? options.Except(assetCategoriesInPlytix.Select(x => x.Name)) : options;

                    // Update asset categories in Plytix
                    var updateAssetCategoryResponse = await UpdateAssetCategoryAsync(plytixInstance, label, newOptions);

                    if (updateAssetCategoryResponse.Succeeded)
                    {
                        string description = label == NavObjectCategory.Collection ? TimeLineDescription.CollectionAcUpdatedSuccessfully : TimeLineDescription.DeliveryPeriodAcUpdatedSuccessfully;
                        timeLines.Add(new TimeLineDTO { Description = description, Status = TimeLineStatus.Successfully, DateTime = DateTime.Now });
                    }
                    else
                    {
                        string description = label == NavObjectCategory.Collection ? TimeLineDescription.CollectionAcUpdateError : TimeLineDescription.DeliveryPeriodAcUpdateError;
                        description += updateAssetCategoryResponse.Error;
                        timeLines.Add(new TimeLineDTO { Description = description, Status = TimeLineStatus.Error, DateTime = DateTime.Now });

                        actionResult.Error = updateAssetCategoryResponse.Error;
                        actionResult.Entity = timeLines;
                        return actionResult;
                    }
                }

                actionResult.Entity = timeLines;
                actionResult.Succeeded = true;

                return actionResult;
            }
            catch (Exception ex)
            {
                actionResult.Error = ex.Message;
                return actionResult;
            }
        }

        private async Task<ActionExecutionResult> UpdateProductAttributeAsync(PlytixInstanceDTO plytixInstance, string label, IEnumerable<string> options)
        {
            var actionResult = new ActionExecutionResult();

            if (options == null || !options.Any())
            {
                actionResult.Succeeded = true;
                return actionResult;
            }

            try
            {
                // Get product attribute from the storage
                var productAttribute = await productAttributeRepository.GetByLabelAsync(label, plytixInstance.Plytix.Id.ToString());

                if (productAttribute == null)
                {
                    actionResult.Error = $"Could not get product attribute with label: {label} for instance {plytixInstance.Plytix.Name}";

                    return actionResult;
                }

                productAttribute.Options = options.ToList();

                // Update product attribute in plytix
                string updateUrl = plytixInstance.Plytix.ServerUrl + "attributes/product/" + productAttribute.Id;

                var updateRequestBody = new ProductAttributeUpdateRequestDTO()
                {
                    Options = productAttribute.Options
                };

                var updateResponse = await this.httpService.PatchAsync<ProductAttributeUpdateRequestDTO, HttpResponse>(updateUrl, updateRequestBody, plytixInstance.Token);

                if (updateResponse.StatusCode != Convert.ToInt32(HttpStatusCode.OK).ToString())
                {
                    actionResult.Error = $"Could not update product attribute with label: {label} for instance {plytixInstance.Plytix.Name}";

                    return actionResult;
                }

                // Update product attribute in the storage
                await productAttributeRepository.UpdateAsync(productAttribute, productAttribute.PlytixInstanceId);

                actionResult.Succeeded = true;

                return actionResult;
            }
            catch (Exception ex)
            {
                actionResult.Error = ex.Message;
                return actionResult;
            }            
        }

        private async Task<ActionExecutionResult> UpdateAssetCategoryAsync(PlytixInstanceDTO plytixInstance, string label, IEnumerable<string> options)
        {
            var actionResult = new ActionExecutionResult();

            string assetCategoryName = label == NavObjectCategory.Collection ? AssetCategoryName.Collection : AssetCategoryName.DeliveryPeriod;

            if (options == null || !options.Any())
            {
                actionResult.Succeeded = true;
                return actionResult;
            }

            try
            {
                // Get asset category from the storage
                var assetCategory = await assetCategoryRepository.GetByNameAsync(assetCategoryName, plytixInstance.Plytix.Id.ToString());

                if (assetCategory == null)
                {
                    actionResult.Error = $"Could not get asset category with name: {assetCategoryName} for instance {plytixInstance.Plytix.Name}";

                    return actionResult;
                }

                // Update asset category in plytix
                string updateUrl = plytixInstance.Plytix.ServerUrl + "categories/file/" + assetCategory.Id;
                var newAssetCategories = new List<AssetCategory>();

                foreach (var option in options)
                {
                    var requestBody = new AssetCategoryUpdateRequestDTO() { Name = option };

                    var updateResponse = await this.httpService.PostAsync<AssetCategoryUpdateRequestDTO, PlytixDataResponseDTO<AssetCategory>>(updateUrl, requestBody, null, plytixInstance.Token);

                    if (string.IsNullOrEmpty(updateResponse?.StatusCode) || updateResponse.StatusCode != Convert.ToInt32(HttpStatusCode.OK).ToString())
                    {
                        actionResult.Error = $"Could not update asset category with name: {assetCategoryName} for instance {plytixInstance.Plytix.Name}";

                        return actionResult;
                    }

                    var assetCategories = updateResponse.Data?.ToList();

                    if (assetCategories?.Count > 0)
                    {
                        newAssetCategories.AddRange(assetCategories);
                    }
                }

                // Update asset categories in the storage
                newAssetCategories.ForEach(x => x.PlytixInstanceId = plytixInstance.Plytix.Id.ToString());

                await assetCategoryRepository.UpdateRangeAsync(newAssetCategories, plytixInstance.Plytix.Id.ToString());

                actionResult.Succeeded = true;

                return actionResult;
            }
            catch (Exception ex)
            {
                actionResult.Error = ex.Message;
                return actionResult;
            }
        }

        private async Task<ActionExecutionResult> SynchronizeProductAttributesAsync(PlytixInstanceDTO plytixInstance)
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                // Get data from API
                string url = plytixInstance.Plytix.ServerUrl + "attributes/product/search";

                var searchRequestBody = new PlytixSearchRequestDTO { Attributes = this.ProductAttibuteFilterAttributes, Pagination = new PaginationDTO { PageSize = DefaultPageSize } };

                var productAttributes = new List<ProductAttribute>();
                int pageNumber = 1;
                int totalRecordsCount;

                do
                {
                    searchRequestBody.Pagination.Page = pageNumber;

                    var content = await this.httpService.PostAsync<PlytixSearchRequestDTO, PlytixSearchResponseDTO<ProductAttribute>>(url, searchRequestBody, null, plytixInstance.Token);

                    if (content?.Data == null || content.StatusCode != Convert.ToInt32(HttpStatusCode.OK).ToString())
                    {
                        actionResult.Entity = content;
                        actionResult.Error = $"Could not get product attributes for instance {plytixInstance.Plytix.Name}";

                        return actionResult;
                    }

                    productAttributes.AddRange(content.Data.ToList());

                    totalRecordsCount = content.Pagination.TotalCount;

                } while (totalRecordsCount / pageNumber++ > DefaultPageSize); // Check if there are more pages

                // Write data to the database
                productAttributes.ForEach(x => x.PlytixInstanceId = plytixInstance.Plytix.Id.ToString());

                await productAttributeRepository.UpdateRangeAsync(productAttributes, plytixInstance.Plytix.Id.ToString());

                actionResult.Succeeded = true;

                return actionResult;
            }
            catch(Exception ex)
            {
                actionResult.Error = ex.Message;
                return actionResult;
            }
        }

        private async Task<ActionExecutionResult> SynchronizeAssetCategoriesAsync(PlytixInstanceDTO plytixInstance)
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                // Get data from API
                string url = plytixInstance.Plytix.ServerUrl + "categories/file/search";

                var searchRequestBody = new PlytixSearchRequestDTO { Attributes = this.AssetCategoryFilterAttributes, Pagination = new PaginationDTO { PageSize = DefaultPageSize } };

                var assetCategories = new List<AssetCategory>();
                int pageNumber = 1;
                int totalRecordsCount;

                do
                {
                    searchRequestBody.Pagination.Page = pageNumber;

                    var content = await this.httpService.PostAsync<PlytixSearchRequestDTO, PlytixSearchResponseDTO<AssetCategory>>(url, searchRequestBody, null, plytixInstance.Token);

                    if (content?.Data == null || content.StatusCode != Convert.ToInt32(HttpStatusCode.OK).ToString())
                    {
                        actionResult.Entity = content;
                        actionResult.Error = $"Could not get asset categories for instance {plytixInstance.Plytix.Name}";

                        return actionResult;
                    }

                    assetCategories.AddRange(content.Data.ToList());

                    totalRecordsCount = content.Pagination.TotalCount;

                } while (totalRecordsCount / pageNumber++ > DefaultPageSize); // Check if there are more pages

                // Write data to the database
                assetCategories.ForEach(x => x.PlytixInstanceId = plytixInstance.Plytix.Id.ToString());

                await assetCategoryRepository.UpdateRangeAsync(assetCategories, plytixInstance.Plytix.Id.ToString());

                actionResult.Succeeded = true;
                actionResult.Entity = assetCategories;

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
