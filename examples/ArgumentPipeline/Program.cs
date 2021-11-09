using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications;

namespace ArgumentPipeline
{
    class Program
    {
        static Task Main(string[] args)
        {
            var hostBuilder = ConsoleHostBuilder
                .CreateDefault()
                .HideDefaultLogging()
                .ConfigureProviders(providers =>
                {
                    // Print help right away
                    providers.AddArguments(new[]{"help"});
                    
                    // Let user type things in the console
                    providers.AddInteractiveConsole(() => Console.Write("ConsoleApps demo > "));
                })
                .Configure(app =>
                {
                    // Quit the app when we encounter these commands
                    app.UseExitCommands(new[]{"quit", "exit"});
                    
                    // Substitute tokens in any arguments with environment variable
                    // values (e.g. $PATH)
                    app.UseEnvironmentVariableTokens();
                    
                    // Substitute tokens in any arguments with the values of Environment.SpecialFolder
                    // values
                    app.UseSpecialFolderTokens();
                    
                    // Display help when requested
                    app.Use((context, next, cancellation) =>
                    {
                        if (context.Arguments.FirstOrDefault() == "help")
                        {
                            Console.WriteLine(
                                "This example program demonstrates how the argument pipeline can be used\n" +
                                "to handle console application arguments in a pipeline. Do any of the following:\n" +
                                "  1. Type 'quit' or 'exit' to stop the application\n" +
                                "  2. Try typing an environment variable (e.g. $PATH, $USER, etc.)\n" +
                                "  3. Try typing the name of a special folder (e.g. $Documents, $LocalApplicationData, tc.)\n");
                        }

                        return next(context, cancellation);
                    });
                    
                    // Print the arguments
                    app.Use((context, next, cancellation) =>
                    {
                        var arguments = context.Arguments;
                        
                        Console.WriteLine($"Arguments ({arguments.Length}):");

                        for (var c = 0; c < arguments.Length; c++)
                        {
                            Console.WriteLine($"  [{c}] = {arguments[c]}");
                        }

                        return next(context, cancellation);
                    });
                });

            return hostBuilder.RunConsoleAsync();
        }
    }
}
