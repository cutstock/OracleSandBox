using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting.Ext.Interfaces;
using Microsoft.Extensions.Hosting.Ext.Services;
using Microsoft.Extensions.Logging;
using OracleSandBox.Worker.Configurations;

namespace OracleSandBox.Worker.Services
{
    internal class WorkerService : HostedServiceBase<CmdOptions>
    {
        private readonly SandBoxBotService _sandBoxBotService;

        public WorkerService(
            ILogger<WorkerService> logger,
            ITaskThrottlerFactory throttlerFactory,
            ICommandLineOptions<CmdOptions> options,
            SandBoxBotService sandBoxBotService)
            : base(logger, throttlerFactory, options)
        {
            _sandBoxBotService = sandBoxBotService;
        }

        protected override async Task ExecuteInternalAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation("Worker executed at: {time}", DateTimeOffset.Now);
            await _sandBoxBotService.StartPollingAsync(stoppingToken);
        }
    }
}
