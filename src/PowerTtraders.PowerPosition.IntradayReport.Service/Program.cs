using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PowerTtraders.PowerPosition.IntradayReport.Lib;
using PowerTtraders.PowerPosition.IntradayReport.Lib.Extensions;
using System.Diagnostics;

namespace PowerTtraders.PowerPosition.IntradayReport.Service;

internal class Program
{
    static void Main(string[] args)
    {
        var isService = !(Debugger.IsAttached || args.Contains("--console"));
        var host = Host.CreateDefaultBuilder(args)
           .UseWindowsService()
            .ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment;

                config
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
         {
             services.AddIntradayReportServices();
             var reportConfig = new ReportConfiguration();
             context.Configuration.GetSection("ReportConfig").Bind(reportConfig);
             services.AddSingleton(reportConfig);
             //services.AddScoped<IIntradayReportGenerator, IntradayReportGenerator>();
             //services.AddScoped<IPowerServiceWrapper, PowerServiceWrapper>();
             //services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

             services.AddHostedService<IntradayReportGeneratorWorkerService>();
         })
         .ConfigureLogging((context, logging) =>
         {
             logging.ClearProviders();
             logging.AddConsole();
             logging.AddDebug();
             logging.AddEventLog(settings =>
             {
                 settings.SourceName = "IntradayReportGeneratorWorkerService";
                 settings.LogName = "Application";
             });
         })
         .Build();

        host.Run();
    }
}
