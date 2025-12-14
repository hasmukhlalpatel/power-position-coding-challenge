
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using PowerTtraders.PowerPosition.IntradayReport.Lib.Extensions;
using PowerTtraders.PowerPosition.IntradayReport.Service;

namespace PowerTtraders.PowerPosition.IntradayReport.Lib.Integration.Tests
{
    public class IntradayReportGeneratorWorkerServiceShould : IAsyncLifetime
    {
        private IHost _host;

        private readonly Mock<ICSVWriter> _mockCSVWriter = new Mock<ICSVWriter>();

        public async Task InitializeAsync()
        {
            _mockCSVWriter
                .Setup(m => m.WriteReport(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()));

            _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true)
                      .AddJsonFile("appsettings.Development.json", optional: true);
            })
            .ConfigureServices((context, services) =>
            {
                var reportConfig = new ReportConfiguration
                {
                    IntervalMinutes = 15,
                    OutputLocation = "./Output"
                };
                services.AddSingleton(reportConfig);
                services.AddIntradayReportServices();
                services.AddSingleton<ICSVWriter>(_mockCSVWriter.Object);
                services.AddHostedService<IntradayReportGeneratorWorkerService>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddDebug();
                logging.AddConsole();
            })
            .Build();
            await _host.StartAsync();
        }

        [Fact]
        public async Task StartAndRunService()
        {
            Assert.NotNull(_host);
            Assert.True(_host.Services != null);

            await Task.Delay(TimeSpan.FromSeconds(3));
            _mockCSVWriter
                .Verify(m => m.WriteReport(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()), 
                Times.AtLeastOnce);
        }

        public async Task DisposeAsync()
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }
        }
    }
}
