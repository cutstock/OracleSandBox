using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting.Ext.Interfaces;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Hosting.Ext
{
    public class TaskThrottler : ITaskThrottler
    {
        private readonly ILogger<TaskThrottler> _logger;

        private readonly List<Task> _tasks = new List<Task>();

        private readonly object _lock = new object();

        private SemaphoreSlim _throttler;

        public TaskThrottler(ILogger<TaskThrottler> logger, int initialThrottleCount, int maxThrottleCount)
        {
            _logger = logger;
            _throttler = new SemaphoreSlim(initialThrottleCount, maxThrottleCount);
        }

        public void Execute(Func<Task> pendingTask, CancellationToken cancellationToken = default)
        {
            Execute(Guid.NewGuid(), pendingTask, cancellationToken);
        }

        public void Execute(Guid trackId, Func<Task> pendingTask, CancellationToken cancellationToken = default)
        {
            if (_throttler == null)
            {
                throw new ObjectDisposedException("SemaphoreSlim");
            }

            var task = Task.Run(
                async () =>
                {
                    _logger.LogTrace("[{trackId}] Waiting for semaphore slim", trackId);
                    await _throttler.WaitAsync(cancellationToken);
                    var watch = new Stopwatch();
                    try
                    {
                        _logger.LogTrace("[{trackId}] Executing task", trackId);
                        watch.Start();
                        await pendingTask();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[{trackId}] Error occured", trackId);
                    }
                    finally
                    {
                        watch.Stop();
                        _throttler.Release();
                        _logger.LogTrace("[{trackId}] Task was completed in {elapsed}", trackId, watch.Elapsed);
                        lock (_lock)
                        {
                            _tasks.RemoveAll(x => x.IsCompleted);
                        }
                    }
                }
            );

            lock (_lock)
            {
                _tasks.Add(task);
            }
        }

        public async Task WaitAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("Waiting all tasks to complete...");
            await Task.WhenAll(_tasks.ToArray());
            lock (_lock)
            {
                _tasks.RemoveAll(x => x.IsCompleted);
            }
        }

        public void Dispose()
        {
            if (_throttler != null)
            {
                _throttler.Dispose();
                _throttler = null;
                _tasks.Clear();
            }
        }
    }
}