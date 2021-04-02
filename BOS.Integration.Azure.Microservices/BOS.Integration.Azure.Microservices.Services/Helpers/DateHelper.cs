using System;
using System.Globalization;

namespace BOS.Integration.Azure.Microservices.Services.Helpers
{
    public static class DateHelper
    {
        private const string ErpDateTimeFormat = "yyyyMMdd hh:mm:ss";
        private const string ErpDateFormat = "yyyyMMdd";
        private const string PrimeCargoFormat = "yyyy-MM-ddTHH:mm:ss";

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
                return date.ToString(PrimeCargoFormat);
            }

            return null;
        }

        public static string ConvertDateTimeToString(DateTime date, string format = null) => date.ToString(format ?? ErpDateTimeFormat);
    }
}
