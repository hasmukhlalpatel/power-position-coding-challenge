using Microsoft.Extensions.DependencyInjection;
using Services;

namespace PowerTtraders.PowerPosition.IntradayReport.Lib.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIntradayReportServices(this IServiceCollection services)
        {
            return services
                .AddTransient<IIntradayReportGenerator, IntradayReportGenerator>()
                .AddTransient<IPowerServiceWrapper, PowerServiceWrapper>()
                .AddTransient<ICSVWriter, CSVWriter>()
                .AddTransient<IDateTimeProvider, DateTimeProvider>()
                .AddTransient<IPowerService, PowerServiceImplementation>();
        }
    }
}
