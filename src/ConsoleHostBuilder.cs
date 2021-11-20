using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vertical.ConsoleApplications.IO;

namespace Vertical.ConsoleApplications
{
    public static class ConsoleHostBuilder
    {
        /// <summary>
        /// Creates a <see cref="IHostBuilder"/> instance specific to
        /// console applications.
        /// </summary>
        /// <returns><see cref="IHostBuilder"/></returns>
        public static IHostBuilder CreateDefault()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<ConsoleHostedService>();
                    services.AddSingleton<IConsoleInputAdapter, DefaultConsoleInputAdapter>();
                });
        }
    }
}