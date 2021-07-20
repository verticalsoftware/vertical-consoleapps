using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Vertical.ConsoleApplications.Providers
{
    /// <summary>
    /// Builder object used to configure argument providers.
    /// </summary>
    public class OrderedProviderBuilder
    {
        private readonly IServiceCollection services;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="services">Services collection</param>
        public OrderedProviderBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        /// <summary>
        /// Adds a provider that provides arguments from a script.
        /// </summary>
        /// <param name="matchPattern">Pattern used to match script files.</param>
        /// <returns>A reference to this instance.</returns>
        public OrderedProviderBuilder AddScriptFile(string matchPattern) => AddProvider(provider =>
            new ScriptFileArgumentsProvider(matchPattern));

        /// <summary>
        /// Adds a provider that handles a static set of arguments.
        /// </summary>
        /// <param name="arguments">Arguments to process.</param>
        /// <returns>A reference to this instance.</returns>
        public OrderedProviderBuilder AddStaticArguments(string[] arguments) => AddProvider(provider =>
            new StaticArgumentsProvider(arguments));

        /// <summary>
        /// Adds an argument provider factory.
        /// </summary>
        /// <param name="factory">Function that builds the provider instance.</param>
        /// <returns>A reference to this instance.</returns>
        public OrderedProviderBuilder AddProvider(Func<IServiceProvider, IArgumentProvider> factory)
        {
            services.AddTransient(factory);
            return this;
        }

        /// <summary>
        /// Adds an argument provider instance.
        /// </summary>
        /// <param name="provider">The provider instance</param>
        /// <returns>A reference to this instance.</returns>
        public OrderedProviderBuilder AddProvider(IArgumentProvider provider)
        {
            services.AddSingleton(provider);
            return this;
        }

        /// <summary>
        /// Adds an argument provider type.
        /// </summary>
        /// <typeparam name="T">The provider type.</typeparam>
        /// <returns>A reference to this instance.</returns>
        public OrderedProviderBuilder AddProvider<T>() where T : class, IArgumentProvider
        {
            services.AddTransient<IArgumentProvider, T>();
            return this;
        }

        /// <summary>
        /// Adds the interactive console provider.
        /// </summary>
        /// <returns></returns>
        public OrderedProviderBuilder AddInteractiveConsole() => AddProvider<InteractiveInputProvider>();
    }
}