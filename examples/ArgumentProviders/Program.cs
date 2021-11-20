using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications;

namespace ArgumentProviders
{
    internal class Program
    {
        private static Task Main(string[] args)
        {
            Environment.SetEnvironmentVariable("DEMO_ENTRY_ARGS", "environment args");
            
            var hostBuilder = ConsoleHostBuilder
                .CreateDefault()
                .ConfigureServices(services => services.AddLogging(logs => logs.ClearProviders()))
                .ConfigureProviders(providers =>
                {
                    // Add input from environment vars
                    providers.AddEnvironmentVariable("DEMO_ENTRY_ARGS");
                    
                    // Add all files in the Scripts
                    providers.AddScripts("Scripts", "*.txt");
                    
                    // Add interactivity
                    providers.AddInteractiveConsole(() => Console.Write("Enter args: "));
                })
                .Configure(app =>
                {
                    app.UseExitCommand("exit");
                    
                    app.UseEnvironmentVariableTokens();

                    app.Use((context, next, cancelToken) =>
                    {
                        Console.WriteLine(context.OriginalFormat);
                        return Task.CompletedTask;
                    });
                });
            
            Console.WriteLine("Type commands or 'exit' to stop the application");

            return hostBuilder.RunConsoleAsync();
        }
    }
}
