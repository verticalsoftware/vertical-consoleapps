using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        /// A delegate that uses the given <see cref="ProviderBuilder"/> to construct
        /// a sequence of argument sources.
        /// </param>
        /// <returns>A reference to the given host builder.</returns>
        public static IHostBuilder ConfigureProviders(
            this IHostBuilder hostBuilder,
            Action<ProviderBuilder> configure)
        {
            hostBuilder.ConfigureServices(services => configure(new ProviderBuilder(services)));
            
            return hostBuilder;
        }

        /// <summary>
        /// Configures the argument processing pipeline.
        /// </summary>
        /// <param name="hostBuilder">Host builder</param>
        /// <param name="configure">
        /// A delegate that uses the given <see cref="IPipelineBuilder"/> to compose the
        /// argument pipeline.
        /// </param>
        /// <returns>A reference to the given host builder.</returns>
        public static IHostBuilder Configure(
            this IHostBuilder hostBuilder,
            Action<IPipelineBuilder> configure)
        {
            hostBuilder.ConfigureServices(services => configure(new PipelineBuilder(services)));
            
            return hostBuilder;
        }

        /// <summary>
        /// Hides logging produced by Microsoft and Vertical infrastructure.
        /// </summary>
        /// <param name="hostBuilder">Host builder</param>
        /// <returns>A reference to the given host builder.</returns>
        public static IHostBuilder HideDefaultLogging(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices(services => services.AddLogging(logging => logging
                .AddFilter("Microsoft.*", _ => false)
                .AddFilter("Vertical.*", _ => false)));
            
            return hostBuilder;
        }
    }
}