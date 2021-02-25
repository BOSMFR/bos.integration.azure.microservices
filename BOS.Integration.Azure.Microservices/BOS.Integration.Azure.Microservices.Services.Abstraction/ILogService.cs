using BOS.Integration.Azure.Microservices.Domain.DTOs;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface ILogService
    {
        Task AddErpMessageAsync(LogInfo erpInfo, string status);

        Task AddTimeLineAsync(LogInfo erpInfo, string description, string status);
    }
}
