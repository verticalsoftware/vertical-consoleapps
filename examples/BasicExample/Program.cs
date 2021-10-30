using System;
using System.IO;
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

                .ConfigureProviders(providers =>
                {
                    // Process the entry arguments
                    providers.AddEntryArguments(entryArgs);
                })
                
                .Configure(app =>
                {
                    app.Use((context, next, cancellation) =>
                    {
                        var arguments = context.Arguments;
                        
                        Console.WriteLine("This example simply prints the arguments received from the command line (Main(string[] args).");
                        Console.WriteLine("Received argument count = {0}", arguments.Length);

                        for(var c = 0; c < arguments.Length; c++)
                        {
                            Console.WriteLine($" [{c}] = {arguments[c]}");
                        }
                        
                        return Task.CompletedTask;
                    });
                });
            

            return host.RunConsoleAsync();
        }
    }
}
