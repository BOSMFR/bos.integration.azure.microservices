using System;
using System.Globalization;

namespace BOS.Integration.Azure.Microservices.Services.Helpers
{
    public static class DateTimeHelper
    {
        public const string ErpDateTimeFormat = "yyyyMMdd hh:mm:ss";
        public const string ErpDateFormat = "yyyyMMdd";
        public const string PrimeCargoDateFormat = "yyyy-MM-ddTHH:mm:ss";

        public static DateTime? ConvertStringToDateTime(string strDate, string format = null)
        {
            if (DateTime.TryParseExact(strDate, format ?? ErpDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date;
            }

            return null;
        }

        public static string ConvertErpDateToPrimeCargoDate(string strDate, string format = null)
        {
            if (DateTime.TryParseExact(strDate, format ?? ErpDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date.ToString(PrimeCargoDateFormat);
            }

            return null;
        }

        public static DateTime ConvertUtcToSpecificTimeZone(DateTime date, string timeZone)
        {
            try
            {
                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);

                var utc = date.ToUniversalTime();

                return TimeZoneInfo.ConvertTimeFromUtc(utc, cstZone);
            }
            catch (Exception ex)
            {
                return date;
            }
        }
    }
}
