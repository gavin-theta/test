using System;

namespace WelNetworks.BidWel.Portal.Core.Extensions
{
    public static class DateTimeExtensions
    {

        public static DateTime ToLocalDateTimeFromUtc(this DateTime dateTimeUtc)
        {

            var timeZoneId = "New Zealand Standard Time";

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var localDateTime = TimeZoneInfo.ConvertTime(dateTimeUtc, TimeZoneInfo.Utc, timeZone);

            return localDateTime;
        }

    }
}
