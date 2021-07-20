using Microsoft.Extensions.Hosting;

namespace Vertical.ConsoleApplications
{
    public static class ConsoleHostBuilder
    {
        /// <summary>
        /// Creates a new <see cref="IHostBuilder"/> instance with default
        /// settings and behavior.
        /// </summary>
        /// <returns><see cref="IHostBuilder"/></returns>
        public static IHostBuilder CreateDefault()
        {
            return new HostBuilder()
                .ConfigureServices(services => services
                        
                    // Adds the main service class    
                    .AddConsoleHost()
                );
        }
    }
}