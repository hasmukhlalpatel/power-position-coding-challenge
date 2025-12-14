using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PowerTtraders.PowerPosition.IntradayReport.Lib;

namespace PowerTtraders.PowerPosition.IntradayReport.Service;

//NOTE : I would prefer to use Worker Service template but keeping it simple for now instead of ServiceBase

public class IntradayReportGeneratorWorkerService : BackgroundService
{
    private readonly ILogger<IntradayReportGeneratorWorkerService> _logger;
    private readonly IIntradayReportGenerator _intradayReportGenerator;
    private readonly ReportConfiguration _reportConfiguration;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public IntradayReportGeneratorWorkerService(
        ILogger<IntradayReportGeneratorWorkerService> logger,
        IIntradayReportGenerator intradayReportGenerator,
        ReportConfiguration reportConfiguration,
        IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;
        _intradayReportGenerator = intradayReportGenerator;
        _reportConfiguration = reportConfiguration;
        _applicationLifetime = applicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Windows Service is starting.");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Service is running at: {time}", DateTimeOffset.Now);

                _ = _intradayReportGenerator.GenerateReportAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        _logger.LogError(task.Exception, "Error in background work");
                    }
                }, stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(_reportConfiguration.IntervalMinutes), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Service cancellation requested.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred in the service.");
            _applicationLifetime.StopApplication();
        }

        _logger.LogInformation("Windows Service is stopping.");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cleanup during service stop.");
        await base.StopAsync(cancellationToken);
    }
}