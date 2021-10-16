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
        /// A delegate that uses the given <see cref="IPipelineBuilder{TContext}"/> and
        /// a service provider to construct an ordered sequence of middleware that
        /// transforms or handles arguments.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public static IHostBuilder Configure(
            this IHostBuilder hostBuilder,
            Action<ApplicationPipelineBuilder> configure)
        {
            hostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton(provider =>
                {
                    var pipelineBuilder = new ApplicationPipelineBuilder(provider);
                    configure(pipelineBuilder);

                    return pipelineBuilder.Build();
                });
            });
            return hostBuilder;
        }

        /// <summary>
        /// Configures the argument processing pipeline.
        /// </summary>
        /// <param name="hostBuilder">Host builder</param>
        /// <param name="configure">
        /// A delegate that uses the given <see cref="IPipelineBuilder{TContext}"/> and
        /// a realized service to construct an ordered sequence of middleware that
        /// transforms or handles arguments.
        /// </param>
        /// <typeparam name="TService">
        /// The type of service to realize from the built application service
        /// provider.
        /// </typeparam>
        /// <returns>A reference to this instance.</returns>
        public static IHostBuilder Configure<TService>(
            this IHostBuilder hostBuilder,
            Action<ApplicationPipelineBuilder, TService> configure)
            where TService : class
        {
            hostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton(provider =>
                {
                    var pipelineBuilder = new ApplicationPipelineBuilder(provider);
                    var service = provider.GetRequiredService<TService>();

                    configure(pipelineBuilder, service);

                    return pipelineBuilder.Build();
                });
            });

            return hostBuilder;
        }
    }
}