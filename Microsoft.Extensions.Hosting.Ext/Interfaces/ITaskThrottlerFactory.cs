namespace Microsoft.Extensions.Hosting.Ext.Interfaces
{
    public interface ITaskThrottlerFactory
    {
        ITaskThrottler Create(int initialThrottleCount, int maxThrottleCount);

        ITaskThrottler Create(int throttleCount);
    }
}