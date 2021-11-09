using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Vertical.ConsoleApplications;

namespace HandlerCommandRouting
{
    internal class Program
    {
        private static Task Main(string[] args)
        {
            var hostBuilder = ConsoleHostBuilder
                .CreateDefault()
                .HideDefaultLogging()
                .ConfigureProviders(providers =>
                {
                    providers.AddArguments(new[] { "help" });
                    providers.AddInteractiveConsole();
                })
                .ConfigureServices((_, services) =>
                {
                    services.AddCommandRouting();
                })
                .Configure(app =>
                {
                    app.Use(async (context, next, cancellation) =>
                    {
                        try
                        {
                            await next(context, cancellation);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                        }
                    });

                    app.UseRouting(router => router.MapHandlers());
                });

            return hostBuilder.RunConsoleAsync();
        }
    }
}
