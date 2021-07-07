using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Ext.Interfaces;

namespace Microsoft.Extensions.Hosting.Ext.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandLineOptions<TCmdOptions>(
            this IServiceCollection services,
            TCmdOptions options
        )
            where TCmdOptions : class, new()
        {
            return services
                .AddSingleton(sp => options)
                .AddSingleton(typeof(ICommandLineOptions<>), typeof(CommandLineOptions<>));
        }

        public static IServiceCollection ConfigureThrottler(this IServiceCollection services)
        {
            return services.AddSingleton<ITaskThrottlerFactory, TaskThrottlerFactory>();
        }
    }
}