using System;
using Microsoft.Extensions.Hosting;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Providers;

namespace Vertical.ConsoleApplications
{
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Configures the argument providers.
        /// </summary>
        /// <param name="hostBuilder">Host builder</param>
        /// <param name="configure">
        /// A delegate the uses the given <see cref="ProviderBuilder"/> to construct
        /// a sequence of argument sources.
        /// </param>
        /// <returns><see cref="IHostBuilder"/></returns>
        public static IHostBuilder ConfigureProviders(
            this IHostBuilder hostBuilder,
            Action<ProviderBuilder> configure)
        {
            hostBuilder.ConfigureServices(services => configure(new ProviderBuilder(services)));
            
            return hostBuilder;
        }

        public static IHostBuilder Configure(
            this IHostBuilder hostBuilder,
            Action<IPipelineBuilder> configure)
        {
            hostBuilder.ConfigureServices(services => configure(new PipelineBuilder(services)));
            
            return hostBuilder;
        }
    }
}