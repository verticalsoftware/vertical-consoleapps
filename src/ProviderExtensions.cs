using System;
using Microsoft.Extensions.DependencyInjection;
using Vertical.ConsoleApplications.Providers;

namespace Vertical.ConsoleApplications
{
    public static class ProviderExtensions
    {
        /// <summary>
        /// Configures the command providers.
        /// </summary>
        /// <param name="services">Services collection</param>
        /// <param name="builder">Object used to construct the provider chain</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection ConfigureProviders(this IServiceCollection services,
            Action<ProviderBuilder> builder)
        {
            builder(new ProviderBuilder(services));
            return services;
        }
    }
}