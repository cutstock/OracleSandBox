using System.Threading.Tasks;
using Microsoft.Extensions.Hosting.Ext;
using OracleSandBox.Worker.Configurations;
using OracleSandBox.Worker.IoC;

namespace OracleSandBox.Worker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await HostExt.BuildAndRunAsync<CmdOptions>(
                args,
                (e, o) => HostConfigurationExtension.ConfigureHostServices(e, o)
            );
        }
    }
}
