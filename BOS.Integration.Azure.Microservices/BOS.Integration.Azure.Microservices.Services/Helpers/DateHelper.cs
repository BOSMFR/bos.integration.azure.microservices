using System;
using System.Globalization;

namespace BOS.Integration.Azure.Microservices.Services.Helpers
{
    public static class DateHelper
    {
        private const string Format = "yyyyMMdd hh:mm:ss";

        public static DateTime? ConvertStringToDateTime(string strDate, string format = null)
        {
            if (DateTime.TryParseExact(strDate, format ?? Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date;
            }

            return null;
        }

        public static string ConvertDateTimeToString(DateTime date, string format = null) => date.ToString(format ?? Format);
    }
}
