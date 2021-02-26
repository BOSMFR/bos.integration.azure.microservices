using BOS.Integration.Azure.Microservices.Domain.Constants;
using System;
using System.Globalization;

namespace BOS.Integration.Azure.Microservices.Services.Helpers
{
    public static class PrimeCargoProductHelper
    {
        private const int DescriptionMaxLength = 50;

        public static string GetPrimeCargoIntegrationState(string startDatePrimeCargoExport)
            => (!DateTime.TryParseExact(startDatePrimeCargoExport, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate) || startDate > DateTime.Today)
                ? PrimeCargoIntegrationState.Waiting : PrimeCargoIntegrationState.NotDelivered;

        public static string TrimPrimeCargoProductDescription(string description) 
            => description?.Length > DescriptionMaxLength
                    ? description.Substring(0, DescriptionMaxLength).Trim()
                    : description;
    }
}
