namespace System
{
    internal static class TimeZones
    {
        public static readonly TimeZoneInfo UTC = TimeZoneInfo.Utc;
        public static DateTime ToUTC(this DateTime dateTime) => dateTime.ToTimeZone(UTC);

        public static readonly TimeZoneInfo EST = GetTimeZoneCrossPlatform("Eastern Standard Time", "US/Eastern");
        public static DateTime ToEST(this DateTime dateTime) => dateTime.ToTimeZone(EST);

        public static readonly TimeZoneInfo CST = GetTimeZoneCrossPlatform("Central Standard Time", "US/Indiana-Starke");
        public static DateTime ToCST(this DateTime dateTime) => dateTime.ToTimeZone(CST);

        public static readonly TimeZoneInfo MDT = GetTimeZoneCrossPlatform("Mountain Standard Time", "US/Mountain");
        public static DateTime ToMDT(this DateTime dateTime) => dateTime.ToTimeZone(MDT);

        public static readonly TimeZoneInfo PST = GetTimeZoneCrossPlatform("Pacific Standard Time", "US/Pacific");
        public static DateTime ToPST(this DateTime dateTime) => dateTime.ToTimeZone(PST);

        public static TimeZoneInfo GetTimeZoneCrossPlatform(string windowsTimeZoneId, string unixTimeZoneId)
        {
            if (string.IsNullOrEmpty(windowsTimeZoneId))
            {
                throw new ArgumentException($"'{nameof(windowsTimeZoneId)}' cannot be null or empty.", nameof(windowsTimeZoneId));
            }
            if (string.IsNullOrEmpty(unixTimeZoneId))
            {
                throw new ArgumentException($"'{nameof(unixTimeZoneId)}' cannot be null or empty.", nameof(unixTimeZoneId));
            }

            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);
            }
            catch (Exception)
            {
                // https://github.com/dotnet/runtime/issues/20523
                return TimeZoneInfo.FindSystemTimeZoneById(unixTimeZoneId);
            }
        }

        public static DateTime NewDateTime(this TimeZoneInfo timeZone, int year, int month, int day)
        {
            var tzDateTime = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Unspecified);

            var utcResult = new DateTimeOffset(tzDateTime, timeZone.GetUtcOffset(tzDateTime)).UtcDateTime;

            return utcResult;
        }

        public static DateTime NewDateTime(this TimeZoneInfo timeZone, int year, int month, int day, int hour, int minute, int second)
        {
            var tzDateTime = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);

            var utcResult = new DateTimeOffset(tzDateTime, timeZone.GetUtcOffset(tzDateTime)).UtcDateTime;

            return utcResult;
        }

        public static DateTime NewDateTime(this TimeZoneInfo timeZone, int year, int month, int day, int hour, int minute, int second, int millisecond)
        {
            var tzDateTime = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Unspecified);

            var utcResult = new DateTimeOffset(tzDateTime, timeZone.GetUtcOffset(tzDateTime)).UtcDateTime;

            return utcResult;
        }

        public static DateTime ToTimeZone(this DateTime dateTime, TimeZoneInfo timeZone)
        {
            return TimeZoneInfo.ConvertTime(dateTime, timeZone);
        }
    }
}
