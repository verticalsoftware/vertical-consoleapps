using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Providers;

namespace Vertical.ConsoleApplications
{
    /// <summary>
    /// Defines a default host builder for console application.
    /// </summary>
    public static class ConsoleHostBuilder
    {
        /// <summary>
        /// Creates a <see cref="IHostBuilder"/> with defaults.
        /// </summary>
        /// <param name="args">Entry arguments.</param>
        /// <returns><see cref="IHostBuilder"/></returns>
        public static IHostBuilder CreateDefault(string[] args)
        {
            var contentRoot = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location)
                              ?? Directory.GetCurrentDirectory();
            var environment = Environment.GetEnvironmentVariable("CONSOLE_ENVIRONMENT")
                              ?? "development";

            return new HostBuilder()
                
                .ConfigureAppConfiguration(configuration => configuration
                    .AddJsonFile(Path.Combine(contentRoot, "appsettings.json"), optional: true)
                    .AddJsonFile(Path.Combine(contentRoot, $"appsettings.{environment}.json"), optional: true))
                
                .ConfigureServices(services =>
                {
                    services.Configure<EntryArguments>(options => options.Arguments = args);
                    services.AddHostedService<ConsoleHostedService>();
                })
                
                .ConfigureLogging(logging => logging
                    .SetMinimumLevel(LogLevel.Information)
                    .AddConsole()
                    .AddDebug());
        }
    }
}