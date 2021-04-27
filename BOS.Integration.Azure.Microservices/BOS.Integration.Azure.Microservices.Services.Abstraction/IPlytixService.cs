using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Packshot;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Plytix;
using BOS.Integration.Azure.Microservices.Domain.Entities.Packshot;
using BOS.Integration.Azure.Microservices.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IPlytixService
    {
        Task<PackshotCreateUpdateDTO> CreateOrUpdatePackshotAsync(RequestMessage<PlytixPackshotRequestDTO> messageObject, ILogger log, ActionType actionType);

        Task<PlytixData<PlytixPackshotResponseData>> UpdatePackshotCategoriesAsync(ResponseMessage<PlytixPackshotUpdateCategoryDTO> messageObject, ILogger log);

        Task<PlytixSyncResultDTO> SynchronizePlytixOptionsAsync(IEnumerable<string> collectionOptions, IEnumerable<string> deliveryPeriodOptions);

        Task<ActionExecutionResult> UpdatePlytixOptionsAsync(string label, IEnumerable<string> options, PlytixInstanceDTO plytixData = null);
    }
}
