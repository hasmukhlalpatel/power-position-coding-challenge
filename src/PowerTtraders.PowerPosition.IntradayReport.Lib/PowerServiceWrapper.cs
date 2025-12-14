using Services;

namespace PowerTtraders.PowerPosition.IntradayReport.Lib;

public class PowerServiceWrapper(IPowerService powerService) : IPowerServiceWrapper
{
    private readonly IPowerService powerService = powerService;

    public async Task<IEnumerable<PowerTradeModel>> GetTradesAsync(DateTime date)
    {
        var trades = await powerService.GetTradesAsync(date);
        return trades.Select(trade =>
            new PowerTradeModel(
                trade.Date,
                trade.Periods.Select(period =>
                new PowerPeriodModel(period.Period, period.Volume))
                .ToArray()));
    }
}

public interface IPowerServiceWrapper
{
    Task<IEnumerable<PowerTradeModel>> GetTradesAsync(DateTime date);
}


public record PowerTradeModel(DateTime Date, PowerPeriodModel[] Periods);

public record PowerPeriodModel(int Period, double Volume);
