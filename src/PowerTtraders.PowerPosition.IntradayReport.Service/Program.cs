using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PowerTtraders.PowerPosition.IntradayReport.Lib;
using PowerTtraders.PowerPosition.IntradayReport.Lib.Extensions;
using System.Diagnostics;

namespace PowerTtraders.PowerPosition.IntradayReport.Service;

internal class Program
{
    static void Main(string[] args)
    {
        var isService = !(Debugger.IsAttached || args.Contains("--console"));

        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseWindowsService();
        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        var reportConfig = new ReportConfiguration();
        builder.Configuration.GetSection("ReportConfig").Bind(reportConfig);
        builder.Services.AddSingleton(reportConfig);
        Console.WriteLine($"Intraday Report Service - ReportConfig: {reportConfig.OutputLocation}");
        builder.Services
            .AddIntradayReportServices()
            .AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy());
        builder.Services.AddHostedService<IntradayReportGeneratorWorkerService>();

        var app = builder.Build();

        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Name == "self"
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = _ => true // include all checks
        });

        app.Run();
    }
}
