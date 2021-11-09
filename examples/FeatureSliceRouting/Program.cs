using System.Threading.Tasks;
using FeatureSliceRouting.Middleware;
using Microsoft.Extensions.Hosting;
using Vertical.ConsoleApplications;

namespace FeatureSliceRouting
{
    internal class Program
    {
        private static Task Main(string[] args)
        {
            var hostBuilder = ConsoleHostBuilder
                .CreateDefault()
                .ConfigureServices(services =>
                {
                    services.AddCommandRouting();
                })
                .ConfigureProviders(providers =>
                {
                    providers.AddInteractiveConsole();
                })
                .Configure(app =>
                {
                    app.UseExitCommands(new[] { "exit", "quit" });

                    app.UseMiddleware<PrintExceptionsMiddleware>();

                    app.UseRouting(router => router.MapHandlers());
                });

            return hostBuilder.RunConsoleAsync();
        }
    }
}
