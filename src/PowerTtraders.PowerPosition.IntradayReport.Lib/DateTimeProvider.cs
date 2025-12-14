namespace PowerTtraders.PowerPosition.IntradayReport.Lib;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime LondonLocalTimeNow => TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time")).Date;

    public DateTime LondonLocalTimeToday => TimeZoneInfo.ConvertTime(DateTime.Today, TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time")).Date;
}

public interface IDateTimeProvider
{
    DateTime LondonLocalTimeNow { get; }
    DateTime LondonLocalTimeToday { get; }
}
