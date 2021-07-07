using Microsoft.Extensions.Hosting.Ext.Interfaces;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Hosting.Ext
{
    public class TaskThrottlerFactory : ITaskThrottlerFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public TaskThrottlerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public ITaskThrottler Create(int initialThrottleCount, int maxThrottleCount)
        {
            return CreateInternal(initialThrottleCount, maxThrottleCount);
        }

        public ITaskThrottler Create(int throttleCount)
        {
            return CreateInternal(throttleCount, throttleCount);
        }

        private ITaskThrottler CreateInternal(int initialThrottleCount, int maxThrottleCount)
        {
            var logger = _loggerFactory.CreateLogger<TaskThrottler>();
            return new TaskThrottler(logger, initialThrottleCount, maxThrottleCount);
        }
    }
}