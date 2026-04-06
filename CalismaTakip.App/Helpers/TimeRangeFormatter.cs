using System.Globalization;

namespace CalismaTakip.Helpers;

public static class TimeRangeFormatter
{
    public static string Format(TimeSpan start, TimeSpan end)
    {
        return $"{FormatTime(start)}–{FormatTime(end)}";
    }

    private static string FormatTime(TimeSpan time)
    {
        return DateTime.Today.Add(time).ToString("HH:mm", CultureInfo.InvariantCulture);
    }
}
