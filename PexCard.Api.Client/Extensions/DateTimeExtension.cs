using System;
using PexCard.Api.Client.Const;

namespace PexCard.Api.Client.Extensions
{
    internal static class DateTimeExtension
    {
        public static DateTime ToTimeZone(this DateTime _this, TimeZoneInfo tz)
        {
            return TimeZoneInfo.ConvertTime(_this, tz);
        }

        public static DateTime ToEst(this DateTime _this)
        {
            return _this.ToTimeZone(TimeZoneConst.Est);
        }

        public static string ToDateTimeString(this DateTime _this)
        {
            return _this.ToString("yyyy-MM-ddTHH:mm:ss");
        }
    }
}
