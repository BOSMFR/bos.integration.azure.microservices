using BOS.Integration.Azure.Microservices.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IPlytixService
    {
        Task<ActionExecutionResult> SynchronizeProductAttributesAsync();

        Task<ActionExecutionResult> SynchronizeAssetCategoriesAsync();

        Task<ActionExecutionResult> UpdateProductAttributeOptionsAsync(string attributeLabel, IEnumerable<string> newOptions);
    }
}
