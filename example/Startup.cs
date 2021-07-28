using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Vertical.ConsoleApplications;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;
using static System.Environment;

namespace Vertical.ConsoleApps.Example
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Providers give command arguments to the pipeline. These
            // are executed in the order in which they are registered
            services.ConfigureCommandProviders(providers => providers
                    
                // Evaluate entry arguments
                .AddEntryArguments()
                
                // Add a startup script
                .AddScript(Path.Combine(GetFolderPath(SpecialFolder.ApplicationData), "Vertical", "startup.txt"))
                
                // Add interactivity with the user
                .AddInteractiveConsoleInput(() => Console.Write("Type something > "))
            );

            // Enable services for command routing
            services.AddCommandRouting();

            services.AddLogging(logging => logging
                .SetMinimumLevel(LogLevel.Trace)
                .ClearProviders()
                .AddConsole()
                .AddFilter("Microsoft.Hosting", LogLevel.Warning)
                .AddFilter("Microsoft.Extensions.Hosting", LogLevel.Warning));
        }

        public void Configure(ApplicationBuilder app)
        {
            // Catches and logs any exceptions
            app.UseExceptionLogging();

            app.UseExceptionHandler<RouteNotFoundException>(() => Console.WriteLine(
                "Command not found (type 'help')"));

            // Replace special folder symbols (e.g. LocalApplicationData)
            app.UseEnvironmentFolderReplacements();

            // Replace environment variable symbols (e.g. $APPDATA)
            app.UseEnvironmentVariableReplacements();
            
            // Route commands to decorated methods
            app.UseCommandRouting();
        }
    }
}