namespace ClassLib.Helpers
{
    public class TimeProvider
    {
        public static DateTime GetVietnamNow()
        {
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
        }
    }
}