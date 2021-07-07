using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Microsoft.Extensions.Hosting.Ext
{
    public static class HostExt
    {
        public static async Task BuildAndRunAsync<TCmdOptions>(
            string[] args,
            Action<IHostBuilder, TCmdOptions> configureHostAction
        )
        {
            Console.OutputEncoding = Encoding.UTF8;
            var parser = new Parser(
                s =>
                {
                    s.ParsingCulture = CultureInfo.InvariantCulture;
                    s.HelpWriter = Console.Out;
                }
            );
            await parser
                .ParseArguments<TCmdOptions>(args)
                .WithParsedAsync(
                    options =>
                    {
                        var builder = Host
                            .CreateDefaultBuilder(args)
                            .UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                        configureHostAction(builder, options);
                        return builder.RunConsoleAsync();
                    }
                );
        }
    }
}