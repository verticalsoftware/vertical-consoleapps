using System;
using System.Collections.Generic;
using System.Threading;
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
        static Task Main(string[] entryArgs)
        {
            var host = ConsoleHostBuilder.CreateDefault()

                // Providers feed arguments to the application. The application
                // executes them in the order in which they are registered here.
                .ConfigureProviders(p =>
                {
                    // Add entry arguments
                    p.AddEntryArguments(entryArgs);

                    // Receives argument from user input
                    p.AddInteractiveConsole("Command > ");
                })
                
                // Make services available to handler components
                .ConfigureServices(services =>
                {
                    services.AddLogging(builder =>
                    {
                        builder.SetMinimumLevel(LogLevel.Trace);
                        builder.AddConsole();
                        builder.AddFilter("Microsoft.*", LogLevel.Critical);
                    });

                    services.AddCommandHandlers();
                })
                
                // Configure the pipeline that processes command arguments
                .Configure<ILogger<Program>>((app, logger) =>
                {
                    // When the user types "exit" or "quit", stop the application
                    app.UseExitCommand("exit", "quit");
                    
                    // Replace $ENVIRONMENT_VARIABLES in arguments
                    app.UseEnvironmentVariables();
                    
                    // Replace $SPECIAL_FOLDER paths
                    app.UseSpecialFolders();
                    
                    // Simply print the arguments back to the console
                    app.Use(next => request =>
                    {
                        var requestArgs = request.Arguments;

                        logger.LogDebug("Arguments received ({count}): {arguments}",
                            requestArgs.Count,
                            requestArgs);

                        return next(request);
                    });

                    app.UseCommands(cmd =>
                    {
                        cmd.MapCommands(
                            ("help", (_, cancel) =>
                            {
                                Console.WriteLine("Help requested!");
                                return Task.CompletedTask;
                            }),
                            ("echo", (args, cancel) =>
                            {
                                Console.WriteLine(string.Join(" ", args));
                                return Task.CompletedTask;
                            }));

                        cmd.MapHandlers();
                    });
                });

            return host.RunConsoleAsync();
        }
    }
}
