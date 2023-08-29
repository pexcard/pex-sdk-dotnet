namespace System
{
    internal static class DateTimeExtension
    {
        public static string ToStringISO8601(this DateTime dateTime)
        {
            return dateTime.ToString("o");
        }
    }
}
