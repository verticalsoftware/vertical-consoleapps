using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications;
using Vertical.ConsoleApplications.Pipeline;

namespace InlineCommandRouting
{
    internal class Program
    {
        private static Task Main(string[] args)
        {
            var hostBuilder = ConsoleHostBuilder
                .CreateDefault()
                .ConfigureServices(services => services.AddLogging(logs => logs.ClearProviders()))
                .ConfigureProviders(providers =>
                {
                    providers.AddArguments(new[] { "help" });
                    providers.AddInteractiveConsole();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddCommandRouting();
                })
                .Configure(app =>
                {
                    app.UseExitCommands(new[] { "quit", "exit" });
                    
                    // Catch errors
                    app.Use(async (context, next, cancelToken) =>
                    {
                        try
                        {
                            await next(context, cancelToken);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    });
                    
                    // Use commands
                    app.UseRouting(router =>
                    {
                        router
                            .Map("add", (context, cancelToken) => PrintResult(context, (a, b) => a + b))
                            .Map("sub", (context, cancelToken) => PrintResult(context, (a, b) => a - b))
                            .Map("mul", (context, cancelToken) => PrintResult(context, (a, b) => a * b))
                            .Map("div", (context, cancelToken) => PrintResult(context, (a, b) => a / b))
                            .Map("pow", (context, cancelToken) => PrintResult(context, Math.Pow))
                            .Map("min", (context, cancelToken) => PrintResult(context, Math.Min))
                            .Map("max", (context, cancelToken) => PrintResult(context, Math.Max))
                            .Map("help", (_, _) =>
                            {
                                Console.WriteLine(
                                    "This program demonstrates inline routed commands by pretending to be a calculator.");
                                Console.WriteLine(
                                    "Type a command (add, sub, mul, div, pow, min, max) and two operands - e.g. add 2 5");
                                Console.WriteLine("Type 'quit' or 'exit' to stop the application.");
                                return Task.CompletedTask;
                            })
                            .MapUnmatched(context =>
                            {
                                var command = context.Arguments.FirstOrDefault();
                                if (string.IsNullOrWhiteSpace(command))
                                    return;
                                Console.WriteLine($"Invalid command '{command}' - type help");
                            });
                    });
                });

            return hostBuilder.RunConsoleAsync();
        }

        private static Task PrintResult(RequestContext context, Func<double, double, double> op)
        {
            var args = context.Arguments;
            var operands = Parse(args);
            var result = op(operands.a, operands.b);
            Console.WriteLine(result);

            return Task.CompletedTask;
        }

        private static (double a, double b) Parse(string[] args)
        {
            if (args.Length != 2)
            {
                throw new ApplicationException("Two operands are required");
            }

            if (!double.TryParse(args[0], out var a))
            {
                throw new ApplicationException($"Cannot convert '{args[0]}' to a number.");
            }
            
            if (!double.TryParse(args[1], out var b))
            {
                throw new ApplicationException($"Cannot convert '{args[1]}' to a number.");
            }

            return (a, b);
        }
    }
}
