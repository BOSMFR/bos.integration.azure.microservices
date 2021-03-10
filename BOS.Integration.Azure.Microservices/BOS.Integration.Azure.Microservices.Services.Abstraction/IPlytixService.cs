using BOS.Integration.Azure.Microservices.Domain;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IPlytixService
    {
        Task<ActionExecutionResult> SynchronizeProductAttributesAsync();
    }
}
