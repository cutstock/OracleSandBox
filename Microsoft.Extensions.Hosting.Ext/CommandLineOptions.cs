using Microsoft.Extensions.Hosting.Ext.Interfaces;

namespace Microsoft.Extensions.Hosting.Ext
{
    public class CommandLineOptions<T> : ICommandLineOptions<T> where T : class, new()
    {
        public CommandLineOptions(T options)
        {
            Value = options;
        }

        public T Value { get; }
    }
}