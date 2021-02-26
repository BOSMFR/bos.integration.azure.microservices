using BOS.Integration.Azure.Microservices.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface ILogService
    {
        Task AddErpMessageAsync(LogInfo erpInfo, string status);

        Task AddErpMessagesAsync(LogInfo erpInfo, ICollection<string> statuses);

        Task AddTimeLineAsync(LogInfo erpInfo, string description, string status);

        Task AddTimeLinesAsync(LogInfo erpInfo, ICollection<TimeLineDTO> timeLines);
    }
}
