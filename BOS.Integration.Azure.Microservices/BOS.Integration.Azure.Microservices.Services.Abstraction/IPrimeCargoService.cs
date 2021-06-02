using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PrimeCargo;
using BOS.Integration.Azure.Microservices.Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services.Abstraction
{
    public interface IPrimeCargoService
    {
        Task<PrimeCargoResponseContent<V>> CreateOrUpdatePrimeCargoObjectAsync<T, V>(RequestMessage<T> messageObject, ILogger log, string entityName, ActionType actionType);

        Task<T> GetPrimeCargoObjectAsync<T>(string url, LogInfo erpInfo, ILogger log, string entityName);

        Task<ActionExecutionResult> GetGoodsReceivalsByLastUpdateAsync(DateTime lastUpdate);

        Task<ActionExecutionResult> GetPickOrderCartonsAsync(int pickOrderHeaderId);
    }
}
