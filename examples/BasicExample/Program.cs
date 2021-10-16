using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications;
using Vertical.ConsoleApplications.Pipeline;

namespace BasicExample
{
    class Program
    {
        static Task Main(string[] args)
        {
            var host = ConsoleHostBuilder
                .CreateDefault()
                .ConfigureProviders(p =>
                {
                    // Providers feed arguments to the application. The application
                    // receives them in the order in which they are registered here.

                    // Add entry arguments
                    p.AddEntryArguments(args);

                    // Receives argument from user input
                    p.AddInteractiveConsole("Command > ");
                })
                .ConfigureServices(services =>
                {
                    services.AddLogging(builder =>
                    {
                        builder.SetMinimumLevel(LogLevel.Trace);
                        builder.AddConsole();
                        builder.AddFilter("Microsoft.*", LogLevel.Critical);
                    });
                })
                .Configure<ILogger<Program>>((app, logger) =>
                {
                    // When the user types "exit" or "quit", stop the application
                    app.UseExitCommand("exit", "quit");

                    // Simply print the arguments back to the console
                    app.Use(next => request =>
                    {
                        var requestArgs = request.Arguments;

                        logger.LogDebug("Arguments received ({count}): {arguments}",
                            requestArgs.Count,
                            requestArgs);

                        return next(request);
                    });
                });

            return host.RunConsoleAsync();
        }
    }
}
