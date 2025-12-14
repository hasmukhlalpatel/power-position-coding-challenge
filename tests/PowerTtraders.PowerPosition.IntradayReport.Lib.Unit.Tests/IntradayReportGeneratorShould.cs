using Microsoft.Extensions.Logging;
using Moq;

namespace PowerTtraders.PowerPosition.IntradayReport.Lib.Unit.Tests;

public class IntradayReportGeneratorShould
{
    [Fact]
    public async Task SuccussfullyGenerateReport()
    {
        var date = DateTime.Today;

        var mockLogger = new Mock<ILogger<IntradayReportGenerator>>();

        var mockPowerService = new Mock<IPowerServiceWrapper>();
        mockPowerService
            .Setup(p => p.GetTradesAsync(It.IsAny<DateTime>()))
            .ReturnsAsync(new List<PowerTradeModel>
            {
                new PowerTradeModel(date, [new PowerPeriodModel(1,100), new PowerPeriodModel(2,100), new PowerPeriodModel(3,100)]),
                new PowerTradeModel(date, [new PowerPeriodModel(1,50), new PowerPeriodModel(2,50), new PowerPeriodModel(3,-20)])
            });

        var dateTimeProvider = new Mock<IDateTimeProvider>();
        dateTimeProvider
            .Setup(d => d.LondonLocalTimeToday)
            .Returns(new DateTime(2025, 12, 14, 0, 0, 0));

        string actualFilename = null;
        List<string> actualDataLines = null;
        var csvWriter = new Mock<ICSVWriter>();
        csvWriter
            .Setup(c => c.WriteReport(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .Callback<string, IEnumerable<string>, CancellationToken>((reportName, dataLines, ct) =>
            {
                actualFilename = reportName;
                actualDataLines = dataLines.ToList();
            });

        var intradayReportGenerator = new IntradayReportGenerator(mockPowerService.Object,
            dateTimeProvider.Object,
            csvWriter.Object,
            mockLogger.Object);

        var exception = await Record.ExceptionAsync(() => intradayReportGenerator.GenerateReportAsync());
        Assert.Null(exception);

        Assert.Equal(4, actualDataLines!.Count);
        Assert.Equal("Local Time,Volume", actualDataLines[0]);
        Assert.Equal("23:00,150", actualDataLines[1]);
        Assert.Equal("00:00,150", actualDataLines[2]);
        Assert.Equal("01:00,80", actualDataLines[3]);

        csvWriter.Verify(c => c.WriteReport(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Once);
    }
}

public class CSVWriterShould
{
    [Fact]
    public void WriteReportToFile()
    {
        var config = new ReportConfiguration { IntervalMinutes = 15, OutputLocation = "./Output" };
        var logger = new Mock<ILogger<CSVWriter>>();

        CSVWriter csvWriter = new(config, logger.Object);
        csvWriter.WriteReport("test.csv", new List<string> { "col1,col2", "data1,data2" });

        //TODO: delete generated file
    }
}