using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting.Ext.Interfaces
{
    public interface ITaskThrottler : IDisposable
    {
        void Execute(Func<Task> pendingTask, CancellationToken cancellationToken = default);

        void Execute(Guid trackId, Func<Task> pendingTask, CancellationToken cancellationToken = default);

        Task WaitAsync(CancellationToken cancellationToken = default);
    }
}