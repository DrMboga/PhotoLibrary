using System.Globalization;

namespace PhotoLibraryBackend.MediaReader;

public static class DateTimeHelper
{
    public static DateTime ToDateTime(this int unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }

    public static int ToUnixTimestamp(this DateTime date)
    {
        var diff = date - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return Convert.ToInt32(diff.TotalSeconds);
    }

    public static DateTime ConvertExifDateStringDateToDate(this string stringDate)
    {
        var dateTime = DateTimeOffset.ParseExact(stringDate, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
        return dateTime.DateTime;
    }
}
