using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Ext.Extensions;
using Microsoft.Extensions.Logging;
using OracleSandBox.Worker.Configurations;
using OracleSandBox.Worker.Services;
using Serilog;
using Serilog.Extensions.Logging;

namespace OracleSandBox.Worker.IoC
{
    internal static class HostConfigurationExtension
    {
        public static IHostBuilder ConfigureHostServices(IHostBuilder builder, CmdOptions options)
        {
            return builder
                .ConfigureLogging(
                    logging =>
                    {
                        logging.ClearProviders();
                        logging.Services.AddSingleton<ILoggerProvider>(
                            sp =>
                            {
                                var hostConfig = sp.GetRequiredService<IConfiguration>();
                                var logger = new LoggerConfiguration()
                                    .ReadFrom.Configuration(hostConfig)
                                    .CreateLogger();
                                return new SerilogLoggerProvider(logger, true);
                            }
                        );
                    }
                )
                .ConfigureServices(
                    (hostContext, services) =>
                    {
                        var configuration = hostContext.Configuration;
                        services.AddCommandLineOptions(options);
                        services.ConfigureThrottler();
                        services.ConfigureServices(configuration, options);
                        services.AddHostedService<WorkerService>();
                    }
                );
        }

        private static IServiceCollection ConfigureServices(
            this IServiceCollection services,
            IConfiguration configuration,
            CmdOptions options)
        {
            services.Configure<TelegramBotConfiguration>(
                e => 
                {
                    e.Token = options.TelegramBotToken;
                });
            services.AddSingleton<SandBoxBotService>();
            return services;
        }
    }
}