using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting.Ext.Interfaces;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Hosting.Ext.Services
{
    public abstract class HostedServiceBase<TCmdOptions> : BackgroundService where TCmdOptions : class, new()
    {
        protected readonly ILogger Logger;

        protected readonly ITaskThrottlerFactory ThrottlerFactory;

        protected readonly TCmdOptions Options;

        protected int? ExitCode;

        protected HostedServiceBase(
            ILogger logger,
            ITaskThrottlerFactory throttlerFactory,
            ICommandLineOptions<TCmdOptions> options
        )
        {
            Logger = logger;
            ThrottlerFactory = throttlerFactory;
            Options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation("Worker executed at: {time}", DateTimeOffset.Now);
            try
            {
                await this.ExecuteInternalAsync(stoppingToken);
                ExitCode = 0;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unhandled exception occured");
                ExitCode = 1;
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogDebug($"Starting with arguments: {string.Join(" ", Environment.GetCommandLineArgs())}");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            // Exit code may be null if the user cancelled via Ctrl+C/SIGTERM
            Environment.ExitCode = ExitCode.GetValueOrDefault(-1);
            Logger.LogDebug($"Exiting with return code: {Environment.ExitCode}");
            return base.StopAsync(cancellationToken);
        }

        protected abstract Task ExecuteInternalAsync(CancellationToken cancellationToken);
    }
}