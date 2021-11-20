using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Providers;
using Vertical.Pipelines.DependencyInjection;

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
        /// A delegate that uses the given <see cref="IPipelineBuilder{TContext}"/> to compose the
        /// argument pipeline.
        /// </param>
        /// <returns>A reference to the given host builder.</returns>
        public static IHostBuilder Configure(
            this IHostBuilder hostBuilder,
            Action<IPipelineBuilder<RequestContext>> configure)
        {
            hostBuilder.ConfigureServices(services => services.ConfigurePipeline(configure));
            
            return hostBuilder;
        }
    }
}