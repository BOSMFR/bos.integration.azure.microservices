using BOS.Integration.Azure.Microservices.Domain;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface INavService
    {
        Task<ActionExecutionResult> UpdateSkuIntoNavAsync(string eanNo, string productId);
    }
}
