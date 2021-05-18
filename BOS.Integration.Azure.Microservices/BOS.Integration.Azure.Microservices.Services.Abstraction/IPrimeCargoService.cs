using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.Enums;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IPrimeCargoService
    {
        Task<Message> CreateOrUpdatePrimeCargoProductAsync(string mySbMsg, ILogger log, ActionType actionType);

        Task<Message> CreateOrUpdatePrimeCargoGoodsReceivalAsync(string mySbMsg, ILogger log, ActionType actionType);

        Task<ActionExecutionResult> GetPrimeCargoGoodsReceivalByIdAsync(string id);

        Task<ActionExecutionResult> GetGoodsReceivalsByLastUpdateAsync(DateTime lastUpdate);
    }
}
