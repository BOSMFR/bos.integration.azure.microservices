using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.DTOs.Plytix;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IPlytixService
    {
        Task<PlytixSyncResultDTO> SynchronizePlytixOptionsAsync(IEnumerable<string> collectionOptions, IEnumerable<string> deliveryPeriodOptions);

        Task<ActionExecutionResult> UpdatePlytixOptionsAsync(string label, IEnumerable<string> newOptions, List<PlytixInstanceDTO> plytixInstances = null);
    }
}
