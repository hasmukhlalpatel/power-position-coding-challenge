using Microsoft.Extensions.DependencyInjection;
using Services;

namespace PowerTtraders.PowerPosition.IntradayReport.Lib.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIntradayReportServices(this IServiceCollection services)
        {
            return services.AddScoped<IIntradayReportGenerator, IntradayReportGenerator>()
                .AddScoped<IPowerServiceWrapper, PowerServiceWrapper>()
                .AddScoped<ICSVWriter, CSVWriter>()
                .AddScoped<IDateTimeProvider, DateTimeProvider>()
                .AddScoped<IPowerService, PowerServiceImplementation>();
        }
    }
}
