using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Hosting.Ext.Interfaces
{
    public interface ICommandLineOptions<T> : IOptions<T> where T : class, new() { }
}