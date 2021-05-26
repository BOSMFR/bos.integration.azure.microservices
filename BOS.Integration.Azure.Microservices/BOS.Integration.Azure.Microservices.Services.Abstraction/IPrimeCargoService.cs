﻿using BOS.Integration.Azure.Microservices.Domain;
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

        Task<ActionExecutionResult> GetGoodsReceivalsByLastUpdateAsync(DateTime lastUpdate);

        Task<ActionExecutionResult> CallPrimeCargoPostEndpointAsync<T, V>(string url, T primeCargoRequestObject);

        Task<ActionExecutionResult> CallPrimeCargoGetEndpointAsync<T>(string url);
    }
}
