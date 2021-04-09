using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Plytix;
using BOS.Integration.Azure.Microservices.Domain.Enums;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IPlytixService
    {
        Task<Message> CreateOrUpdatePackshotAsync(string mySbMsg, ILogger log, ActionType actionType);

        Task<PlytixSyncResultDTO> SynchronizePlytixOptionsAsync(IEnumerable<string> collectionOptions, IEnumerable<string> deliveryPeriodOptions);

        Task<ActionExecutionResult> UpdatePlytixOptionsAsync(string label, IEnumerable<string> newOptions, List<PlytixInstanceDTO> plytixInstances = null);
    }
}
