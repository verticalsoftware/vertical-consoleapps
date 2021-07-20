using Microsoft.Extensions.DependencyInjection;

namespace Vertical.ConsoleApplications
{
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Adds the console hosted service.
        /// </summary>
        /// <param name="services">Services collection</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddConsoleHost(this IServiceCollection services)
        {
            services.AddHostedService<ConsoleService>();
            return services;
        }
    }
}