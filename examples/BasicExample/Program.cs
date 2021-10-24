using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications;

namespace BasicExample
{
    class Program
    {
        static Task Main(string[] entryArgs)
        {
            var host = ConsoleHostBuilder.CreateDefault()
                
                .ConfigureServices(services =>
                {
                    // Silence Microsoft logging
                    services.AddLogging(logging => logging
                        .AddFilter("Microsoft.*", LogLevel.Critical)
                        .SetMinimumLevel(LogLevel.Trace));

                    services.AddCommandRouting();
                })

                .ConfigureProviders(providers =>
                {
                    //providers.AddEntryArguments(entryArgs);
                    
                    // Let user type input to our program
                    providers.AddInteractiveConsole(() => Console.Write("Type something or 'exit' > "));
                })
                
                .Configure(app =>
                {
                    app.UseExitCommands(new[]{"exit", "quit"});

                    app.UseEnvironmentVariableTokens();

                    app.UseSpecialFolderTokens();
                    
                    app.Use(async (context, next, cancelToken) =>
                    {
                        await Task.Delay(250, CancellationToken.None);
                        
                        Console.WriteLine(string.Join(' ', context.Arguments));

                        await next(context, cancelToken);
                    });

                    app.UseRouting(router =>
                    {
                        router.Map("help", (context, ct) =>
                        {
                            Console.WriteLine("You need help!");
                            return Task.CompletedTask;
                        });

                        router.Map("backup", (context, ct) =>
                        {
                            Console.WriteLine("You want to backup {0} to {1}",
                                context.Arguments[0],
                                context.Arguments[1]);
                            return Task.CompletedTask;
                        });
                    });
                });

            return host.RunConsoleAsync();
        }
    }
}
