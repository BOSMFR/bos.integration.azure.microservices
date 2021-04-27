using AutoMapper;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Auth;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Plytix;
using BOS.Integration.Azure.Microservices.Domain.Entities.Packshot;
using BOS.Integration.Azure.Microservices.Domain.Entities.Plytix;
using BOS.Integration.Azure.Microservices.Domain.Enums;
using BOS.Integration.Azure.Microservices.Infrastructure.Configuration;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IAssetCategoryService assetCategoryService;
        private readonly IHttpService httpService;
        private readonly ILogService logService;
        private readonly IProductAttributeRepository productAttributeRepository;
        private readonly IAssetCategoryRepository assetCategoryRepository;
        private readonly IPlytixRepository plytixRepository;
        private readonly IMapper mapper;

        public PlytixService(
            IConfigurationManager configuration,
            IAssetCategoryService assetCategoryService,
            IHttpService httpService,
            ILogService logService,
            IProductAttributeRepository productAttributeRepository,
            IAssetCategoryRepository assetCategoryRepository,
            IPlytixRepository plytixRepository,
            IMapper mapper)
        {
            this.configuration = configuration;
            this.assetCategoryService = assetCategoryService;
            this.httpService = httpService;
            this.logService = logService;
            this.productAttributeRepository = productAttributeRepository;
            this.assetCategoryRepository = assetCategoryRepository;
            this.plytixRepository = plytixRepository;
            this.mapper = mapper;

            this.ProductAttibuteFilterAttributes = new List<string> { "name", "label", "attributes", "groups", "type_class", "options" };
            this.AssetCategoryFilterAttributes = new List<string> { "name", "n_children", "path", "parents_ids", "order" };
        }

        public async Task<PackshotCreateUpdateDTO> CreateOrUpdatePackshotAsync(RequestMessage<PlytixPackshotRequestDTO> messageObject, ILogger log, ActionType actionType)
        {
            var erpMessageStatuses = new List<string>();
            var timeLines = new List<TimeLineDTO>();

            // Add erp messages and time lines
            erpMessageStatuses.Add(actionType == ActionType.Create ? ErpMessageStatus.CreateMessage : ErpMessageStatus.UpdateMessage);

            timeLines.Add(new TimeLineDTO
            {
                Description = actionType == ActionType.Create ? TimeLineDescription.PackshotCreateMessageSentServiceBus : TimeLineDescription.PackshotUpdateMessageSentServiceBus,
                Status = TimeLineStatus.Information,
                DateTime = DateTime.Now
            });

            // Decode file url
            var fileDto = messageObject.RequestObject.File;
            fileDto.Url = Uri.UnescapeDataString(fileDto.Url);

            // Get plytix instance by product brand
            var plytixInstance = await plytixRepository.GetActiveInstanceAsync();

            if (plytixInstance == null)
            {
                erpMessageStatuses.Add(actionType == ActionType.Create ? ErpMessageStatus.ErrorCreatePackshot : ErpMessageStatus.ErrorUpdatePackshot);
                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PlytixInstanceError, Status = TimeLineStatus.Error, DateTime = DateTime.Now });

                log.LogError(TimeLineDescription.PlytixInstanceError);

                // Write erp messages and time lines to database
                await this.logService.AddErpMessagesAsync(messageObject.ErpInfo, erpMessageStatuses);
                await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

                return null;
            }

            // Get access token
            var authBody = new PlytixAuthRequestDTO
            {
                Key = plytixInstance.Key,
                Password = plytixInstance.Paswword
            };

            var authResponse = await this.httpService.PostAsync<PlytixAuthRequestDTO, PlytixAuthResponseDTO>(configuration.PlytixSettings.AuthUrl, authBody);

            string token = authResponse?.Data?.FirstOrDefault()?.AccessToken;

            if (string.IsNullOrEmpty(token))
            {
                erpMessageStatuses.Add(actionType == ActionType.Create ? ErpMessageStatus.ErrorCreatePackshot : ErpMessageStatus.ErrorUpdatePackshot);
                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PlytixTokenError + plytixInstance.Name, Status = TimeLineStatus.Error, DateTime = DateTime.Now });

                log.LogError(TimeLineDescription.PlytixTokenError + plytixInstance.Name);

                // Write erp messages and time lines to database
                await this.logService.AddErpMessagesAsync(messageObject.ErpInfo, erpMessageStatuses);
                await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

                return null;
            }

            // Use Plytix API to process the object 
            string url = plytixInstance.ServerUrl + "assets";

            var response = await this.httpService.PostAsync<FileDTO, PlytixPackshotResponseDTO>(url, fileDto, null, token);

            // Check response and write logs
            if (response?.StatusCode == Convert.ToInt32(HttpStatusCode.RequestTimeout).ToString())
            {
                erpMessageStatuses.Add(actionType == ActionType.Create ? ErpMessageStatus.PlytixCreateTimeout : ErpMessageStatus.PlytixUpdateTimeout);
                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PlytixRequestTimeOut, Status = TimeLineStatus.Error, DateTime = DateTime.Now });

                // Write erp messages and time lines to database
                await this.logService.AddErpMessagesAsync(messageObject.ErpInfo, erpMessageStatuses);
                await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

                throw new Exception("Request time out");
            }

            if (response.StatusCode != Convert.ToInt32(HttpStatusCode.OK).ToString() && response.StatusCode != Convert.ToInt32(HttpStatusCode.Created).ToString())
            {
                string error = actionType == ActionType.Create ? TimeLineDescription.ErrorCreatePackshotPlytix : TimeLineDescription.ErrorUpdatePackshotPlytix;

                var responseError = JsonConvert.DeserializeObject<PlytixPackshotResponseErrorDTO>(response.ErrorObject);

                error += responseError.Error.Msg;

                erpMessageStatuses.Add(actionType == ActionType.Create ? ErpMessageStatus.ErrorCreatePackshot : ErpMessageStatus.ErrorUpdatePackshot);
                timeLines.Add(new TimeLineDTO { Description = error, Status = TimeLineStatus.Error, DateTime = DateTime.Now });

                log.LogError(error);

                // Write erp messages and time lines to database
                await this.logService.AddErpMessagesAsync(messageObject.ErpInfo, erpMessageStatuses);
                await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines); 

                return null;
            }

            erpMessageStatuses.Add(ErpMessageStatus.PlytixDeliveredSuccessfully);
            timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PlytixDeliveredSuccessfully, Status = TimeLineStatus.Successfully, DateTime = DateTime.Now });

            // Write erp messages and time lines to database
            await this.logService.AddErpMessagesAsync(messageObject.ErpInfo, erpMessageStatuses);
            await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

            // Create update categories request object
            var plytixResponseObject = new PlytixData<PlytixPackshotResponseData> { Data = response?.Data };

            var packshotUpdateCategoryDTO = this.mapper.Map<PlytixPackshotUpdateCategoryDTO>(messageObject.RequestObject);
            packshotUpdateCategoryDTO.Plytix = plytixInstance;
            packshotUpdateCategoryDTO.PlytixToken = token;
            packshotUpdateCategoryDTO.AssetId = plytixResponseObject.Data?.FirstOrDefault()?.Id;

            return new PackshotCreateUpdateDTO { PlytixResponseObject = plytixResponseObject, PackshotUpdateCategoryDTO = packshotUpdateCategoryDTO };            
        }

        public async Task<PlytixData<PlytixPackshotResponseData>> UpdatePackshotCategoriesAsync(ResponseMessage<PlytixPackshotUpdateCategoryDTO> messageObject, ILogger log)
        {
            var erpMessageStatuses = new List<string>();
            var timeLines = new List<TimeLineDTO>();

            var plytixInstance = messageObject.ResponseObject.Plytix;

            // Get asset category ids and create a request object
            var assetCategoryIds = await this.assetCategoryService.GetAssetCategoryIdsByPackshotInfoAsync(messageObject.ResponseObject);

            var requestObject = new PlytixPackshotUpdateRequestDTO { Categories = assetCategoryIds.Select(id => new PlytixPackshotCategoryDTO { Id = id }).ToList() };

            // Update asset
            string url = plytixInstance.ServerUrl + "assets/" + messageObject.ResponseObject.AssetId;

            var response = await this.httpService.PatchAsync<PlytixPackshotUpdateRequestDTO, PlytixPackshotResponseDTO>(url, requestObject, messageObject.ResponseObject.PlytixToken);

            // Check response and write logs
            if (response?.StatusCode == Convert.ToInt32(HttpStatusCode.RequestTimeout).ToString())
            {
                erpMessageStatuses.Add(ErpMessageStatus.PlytixUpdateTimeout);
                timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.PlytixRequestTimeOut, Status = TimeLineStatus.Error, DateTime = DateTime.Now });

                // Write erp messages and time lines to database
                await this.logService.AddErpMessagesAsync(messageObject.ErpInfo, erpMessageStatuses);
                await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

                throw new Exception("Request time out");
            }

            if (response.StatusCode != Convert.ToInt32(HttpStatusCode.OK).ToString())
            {
                var responseError = JsonConvert.DeserializeObject<PlytixAssetCategoryUpdateErrorDTO>(response.ErrorObject);

                string error = TimeLineDescription.ErrorUpdatePackshotCategoryPlytix + responseError.Message;

                erpMessageStatuses.Add(ErpMessageStatus.ErrorUpdatedPackshotCategory);
                timeLines.Add(new TimeLineDTO { Description = error, Status = TimeLineStatus.Error, DateTime = DateTime.Now });

                log.LogError(error);

                // Write erp messages and time lines to database
                await this.logService.AddErpMessagesAsync(messageObject.ErpInfo, erpMessageStatuses);
                await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

                return null;
            }

            erpMessageStatuses.Add(ErpMessageStatus.SuccessfullyUpdatedPackshotCategory);
            timeLines.Add(new TimeLineDTO { Description = TimeLineDescription.SuccsessUpdatePackshotCategoryPlytix, Status = TimeLineStatus.Successfully, DateTime = DateTime.Now });

            // Write erp messages and time lines to database
            await this.logService.AddErpMessagesAsync(messageObject.ErpInfo, erpMessageStatuses);
            await this.logService.AddTimeLinesAsync(messageObject.ErpInfo, timeLines);

            return new PlytixData<PlytixPackshotResponseData> { Data = response?.Data };
        }

        public async Task<PlytixSyncResultDTO> SynchronizePlytixOptionsAsync(IEnumerable<string> collectionOptions, IEnumerable<string> deliveryPeriodOptions)
        {
            var actionResult = new PlytixSyncResultDTO();

            try
            {
                var response = await GetPlytixInstanceWithTokenAsync();

                if (!response.Succeeded)
                {
                    actionResult.GeneralResult.Error = response.Error;
                    return actionResult;
                }

                var plytixData = response.Entity as PlytixInstanceDTO;

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

        public async Task<ActionExecutionResult> UpdatePlytixOptionsAsync(string label, IEnumerable<string> options, PlytixInstanceDTO plytixData = null)
        {
            var actionResult = new ActionExecutionResult();
            var timeLines = new List<TimeLineDTO>();

            try
            {
                if (plytixData == null)
                {
                    var response = await GetPlytixInstanceWithTokenAsync();

                    if (!response.Succeeded)
                    {
                        actionResult.Error = response.Error;
                        return actionResult;
                    }

                    plytixData = response.Entity as PlytixInstanceDTO;
                }

                // Synchronize product attributes in the storage with Plytix
                var synchronizeProductAttributesResponse = await SynchronizeProductAttributesAsync(plytixData);

                if (!synchronizeProductAttributesResponse.Succeeded)
                {
                    actionResult.Error = synchronizeProductAttributesResponse.Error;
                    return actionResult;
                }

                // Update product attributes in the storage and Plytix
                var updateProductAttributeResponse = await UpdateProductAttributeAsync(plytixData, label, options);

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
                var synchronizeAssetCategoriesResponse = await this.SynchronizeAssetCategoriesAsync(plytixData);

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
                var updateAssetCategoryResponse = await UpdateAssetCategoryAsync(plytixData, label, newOptions);

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

        #region Private methods

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
            catch (Exception ex)
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

        private async Task<ActionExecutionResult> GetPlytixInstanceWithTokenAsync()
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                // Get plytix instances from the storage
                var plytixInstance = await this.plytixRepository.GetActiveInstanceAsync();

                if (plytixInstance == null)
                {
                    actionResult.Error = TimeLineDescription.PlytixInstanceError;

                    return actionResult;
                }

                // Get access token
                var authBody = new PlytixAuthRequestDTO
                {
                    Key = plytixInstance.Key,
                    Password = plytixInstance.Paswword
                };

                var authResponse = await this.httpService.PostAsync<PlytixAuthRequestDTO, PlytixAuthResponseDTO>(configuration.PlytixSettings.AuthUrl, authBody);

                string token = authResponse?.Data?.FirstOrDefault()?.AccessToken;

                if (string.IsNullOrEmpty(token))
                {
                    actionResult.Entity = authResponse;
                    actionResult.Error = $"Could not get access token for instance {plytixInstance.Name}";

                    return actionResult;
                }

                actionResult.Entity = new PlytixInstanceDTO { Plytix = plytixInstance, Token = token };
                actionResult.Succeeded = true;

                return actionResult;
            }
            catch (Exception ex)
            {
                actionResult.Error = ex.Message;
                return actionResult;
            }
        }

        #endregion

    }
}
