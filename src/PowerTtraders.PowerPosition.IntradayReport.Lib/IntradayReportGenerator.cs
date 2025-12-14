using Microsoft.Extensions.Logging;
using System.Globalization;

namespace PowerTtraders.PowerPosition.IntradayReport.Lib;

public interface IIntradayReportGenerator
{
    Task GenerateReportAsync(CancellationToken stoppingToken = default);
}

public class IntradayReportGenerator(
    IPowerServiceWrapper powerService,
    IDateTimeProvider dateTimeProvider,
    ICSVWriter csvWriter,
    ILogger<IntradayReportGenerator> logger) : IIntradayReportGenerator
{
    public async Task GenerateReportAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Generating intraday report...");
            var localTime = dateTimeProvider.LondonLocalTimeToday;

            if (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Cancellation requested - skipping work");
                cancellationToken.ThrowIfCancellationRequested();
            }

            var trades = await powerService.GetTradesAsync(localTime);

            // Prepare CSV data : can send into StringBuilder but keeping it simple for now
            var reportLines = new List<string>
            {
                "Local Time,Volume"
            };

            var hourlyVolumes = trades
                    .SelectMany(t => t.Periods)
                    .GroupBy(p => p.Period)
                    .Select(g => new
                    {
                        Hour = g.Key - 1, // Period 1 = 00:00
                        Volume = g.Sum(x => x.Volume)
                    })
                    .OrderBy(x => x.Hour)
                    .ToList();

            logger.LogDebug("Found Hourly Volumes: {@HourlyVolumes}", hourlyVolumes.Count);

            foreach (var hv in hourlyVolumes)
            {
                var localStart = localTime.AddHours(hv.Hour - 1);
                var timeStr = localStart.ToString("HH:mm", CultureInfo.InvariantCulture);
                reportLines.Add($"{timeStr},{hv.Volume}");
            }

            string fileName = $"PowerPosition_{localTime:yyyyMMdd}_{localTime:HHmm}.csv";
            csvWriter.WriteReport(fileName, reportLines, cancellationToken);

            logger.LogInformation("Intraday report generation completed.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating intraday report.");
            //TODO: REVIEW requred : Handle exception as needed and rethrowing for now. It might write twice logs
            throw;
        }
    }
}
