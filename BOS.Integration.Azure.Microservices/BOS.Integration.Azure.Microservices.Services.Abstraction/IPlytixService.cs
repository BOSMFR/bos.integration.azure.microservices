using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.Entities.Collection;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IPlytixService
    {
        Task<ActionExecutionResult> SynchronizeProductAttributesAsync();

        Task<ActionExecutionResult> UpdateCollectionProductAttributeAsync(CollectionEntity collection);
    }
}
