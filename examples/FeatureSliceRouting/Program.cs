using System;
using System.Threading.Tasks;
using FeatureSliceRouting.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications;

namespace FeatureSliceRouting
{
    internal class Program
    {
        private static Task Main(string[] args)
        {
            var hostBuilder = ConsoleHostBuilder
                .CreateDefault()
                .HideDefaultLogging()
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
            
            Console.WriteLine("This demo program demonstrates feature slicing.");
            Console.WriteLine("The following commands are supported:");
            Console.WriteLine("  cat <path> - prints the contents of a file");
            Console.WriteLine("  copy <src> <dest> - copies a file from one location to another");
            Console.WriteLine("  sort <path> [asc|desc] - prints the content lines of a file in sorted order");
            Console.WriteLine("  exit - stops the application");

            return hostBuilder.RunConsoleAsync();
        }
    }
}
