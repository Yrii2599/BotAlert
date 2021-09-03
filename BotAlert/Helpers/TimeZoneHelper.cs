using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotAlert.Helpers
{
    public static class TimeZoneHelper
    {
        public static string PrintTimeZone(int timeZone)
        {
            if (timeZone>0)
            {
                return $"Текущий часовой пояс: UTC +{timeZone}:00 \n";
            }
            else
            {
                return $"Текущий часовой пояс: UTC {timeZone}:00 \n";
            }
        }
    }
}
