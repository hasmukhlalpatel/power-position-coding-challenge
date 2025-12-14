using Services;

namespace PowerTtraders.PowerPosition.IntradayReport.Lib;

public class PowerServiceImplementation : IPowerService
{
    public IEnumerable<PowerTrade> GetTrades(DateTime date)
    {
        return new List<PowerTrade>
        {
            PowerTrade.Create(date,10),
        };
    }

    public Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date)
    {
        return Task.FromResult(GetTrades(date));
    }
}