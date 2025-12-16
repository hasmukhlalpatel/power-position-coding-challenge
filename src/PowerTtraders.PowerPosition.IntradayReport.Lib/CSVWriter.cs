using Microsoft.Extensions.Logging;

namespace PowerTtraders.PowerPosition.IntradayReport.Lib;

public class CSVWriter(ReportConfiguration configuration, ILogger<CSVWriter> logger) : ICSVWriter
{
    public void WriteReport(string reportName, IEnumerable<string> dataLines, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(configuration.OutputLocation, reportName);
        logger.LogInformation("Writing report to {filePath}...", filePath);

        //NOTE: can use File.AppendAllLines() but StreamWriter is a better way to write large file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var line in dataLines)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("Cancellation requested - skipping work");
                    cancellationToken.ThrowIfCancellationRequested();
                }
                writer.WriteLine(line);
            }
        }

        logger.LogInformation("Report written to {reportName}.", reportName);
    }
}

public interface ICSVWriter { 
    void WriteReport(string reportName, IEnumerable<string> dataLines, CancellationToken cancellationToken = default);
}