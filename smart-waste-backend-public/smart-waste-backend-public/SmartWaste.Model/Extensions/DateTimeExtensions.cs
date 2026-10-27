namespace SmartWaste.Model.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToJsonFormat(this DateTime dateTime)
    {
        var trimmed = dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond));
        return DateTime.SpecifyKind(trimmed, DateTimeKind.Unspecified);
    }


    public static DateTime? ToJsonFormat(this DateTime? dateTime)
    {
        if (!dateTime.HasValue) return null;
        return dateTime.Value.ToJsonFormat();
    }
}