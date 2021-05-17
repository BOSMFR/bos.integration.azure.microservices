using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Functions.GoodsReceival
{
    public class GoodsReceivalTimerFunction
    {
        private readonly IPrimeCargoService primeCargoService;

        public GoodsReceivalTimerFunction(IPrimeCargoService primeCargoService)
        {
            this.primeCargoService = primeCargoService;
        }

        [FixedDelayRetry(5, "00:05:00")]
        [FunctionName("GoodsReceivalTimerFunction")]
        public async Task Run([TimerTrigger("0 0 1 * * *")] TimerInfo myTimer, ILogger log)
        {
            try
            {
                log.LogInformation($"GoodsReceivalTimer function executed at: {DateTime.Now}");

            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
