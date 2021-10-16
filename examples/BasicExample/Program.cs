using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications;

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
                    p.AddInteractiveConsole();
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
                .Configure((services, app) =>
                {
                    app.Use(next => request =>
                    {
                        var logger = services.GetService<ILogger<Program>>();
                        var args = request.Arguments;
                        
                        Console.WriteLine("Arguments received:");
                        logger.LogDebug("Arguments received ({count}): {arguments}", 
                            args.Count,
                            args);

                        foreach (var arg in args)
                        {
                            Console.WriteLine($"   {arg}");
                        }

                        return next(request);
                    });
                })
                .Build();

            return host.RunAsync();
        }
    }
}
