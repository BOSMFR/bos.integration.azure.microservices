using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace BOS.Integration.Azure.Microservices.Functions
{
    public static class SkuTimerFunction
    {
        [FunctionName("SkuTimerFunction")]
        public static void Run([TimerTrigger(" 0 0 1 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"SkuTimer function executed at: {DateTime.Now}");
        }
    }
}
