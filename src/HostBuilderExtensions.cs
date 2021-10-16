using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Providers;
using Vertical.Pipelines;

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

        /// <summary>
        /// Configures the argument processing pipeline.
        /// </summary>
        /// <param name="hostBuilder">Host builder</param>
        /// <param name="configure">
        /// A delegate that uses the given <see cref="IPipelineBuilder{TContext}"/> to
        /// construct an ordered sequence of middleware that transforms or handles
        /// arguments.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public static IHostBuilder Configure(
            this IHostBuilder hostBuilder,
            Action<IPipelineBuilder<ArgumentsContext>> configure)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                var pipelineBuilder = new PipelineBuilder<ArgumentsContext>();
                configure(pipelineBuilder);

                services.AddSingleton(pipelineBuilder.Build());
            });
        }

        /// <summary>
        /// Configures the argument processing pipeline.
        /// </summary>
        /// <param name="hostBuilder">Host builder</param>
        /// <param name="configure">
        /// A delegate that uses the given <see cref="IPipelineBuilder{TContext}"/> and
        /// a service provider to construct an ordered sequence of middleware that
        /// transforms or handles arguments.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public static IHostBuilder Configure(
            this IHostBuilder hostBuilder,
            Action<IServiceProvider, IPipelineBuilder<ArgumentsContext>> configure)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton(provider =>
                {
                    var pipelineBuilder = new PipelineBuilder<ArgumentsContext>();
                    configure(provider, pipelineBuilder);

                    return pipelineBuilder.Build();
                });
            });
        }
    }
}