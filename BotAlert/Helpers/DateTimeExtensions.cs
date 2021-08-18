using System;

namespace BotAlert.Helpers
{
    public static class DateTimeExtensions
    {
        public static DateTime TrimSecondsAndMilliseconds(this DateTime date)
        {
            return new DateTime(date.Year,
                                date.Month,
                                date.Day,
                                date.Hour,
                                date.Minute,
                                0,
                                0);
        }
    }
}
