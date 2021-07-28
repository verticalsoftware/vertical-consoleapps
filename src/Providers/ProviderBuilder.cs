using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Vertical.ConsoleApplications.Providers
{
    public class ProviderBuilder
    {
        private readonly IServiceCollection services;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="services">Service collection.</param>
        public ProviderBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        /// <summary>
        /// Adds the application entry arguments as a command provider.
        /// </summary>
        /// <returns>A reference to this instance.</returns>
        public ProviderBuilder AddEntryArguments()
        {
            services.AddSingleton<ICommandProvider>(provider => new StaticArgumentsProvider(
                provider.GetRequiredService<IOptions<EntryArguments>>().Value.Arguments,
                provider.GetService<ILogger<StaticArgumentsProvider>>()));
            return this;
        }

        /// <summary>
        /// Adds an interactive console provider.
        /// </summary>
        /// <param name="prompt">Optional action that sets up the prompt to the user.</param>
        /// <returns>A reference to this instance.</returns>
        public ProviderBuilder AddInteractiveConsoleInput(Action? prompt = null)
        {
            services.AddSingleton<ICommandProvider>(provider => new InteractiveConsoleProvider(
                provider.GetService<ILogger<InteractiveConsoleProvider>>(),
                prompt));
            return this;
        }

        /// <summary>
        /// Adds the commands found in a script as a provider.
        /// </summary>
        /// <param name="path">Path to the script file.</param>
        /// <param name="optional">Whether the script is optional.</param>
        /// <returns>A reference to this instance.</returns>
        public ProviderBuilder AddScript(string path, bool optional = true)
        {
            services.AddSingleton<ICommandProvider>(provider => new ScriptFileProvider(
                path, optional, provider.GetService<ILogger<ScriptFileProvider>>()));
            return this;
        }
    }
}