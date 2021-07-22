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
        /// <returns></returns>
        public ProviderBuilder AddEntryArguments()
        {
            services.AddSingleton<ICommandProvider>(provider => new StaticArgumentsProvider(
                provider.GetRequiredService<IOptions<EntryArguments>>().Value.Arguments));
            return this;
        }
    }
}