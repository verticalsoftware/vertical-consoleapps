using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Vertical.ConsoleApplications.Pipeline;

namespace Vertical.ConsoleApplications.Routing
{
    public class RoutingBuilder
    {
        internal RoutingBuilder(IServiceCollection applicationServices)
        {
            ApplicationServices = applicationServices;

            applicationServices.AddSingleton(serviceProvider => new HandlerMap(serviceProvider
                .GetServices<RouteDescriptor>()));
        }

        /// <summary>
        /// Gets the application's service collection.
        /// </summary>
        public IServiceCollection ApplicationServices { get; }

        /// <summary>
        /// Maps a command route to a handler implementation.
        /// </summary>
        /// <param name="route">Route</param>
        /// <param name="handler">
        /// A delegate that implements the command logic
        /// </param>
        /// <returns>A reference to this instance</returns>
        public RoutingBuilder Map(string route,
            Func<CommandContext, CancellationToken, Task> handler)
        {
            ApplicationServices.AddSingleton(new RouteDescriptor(route, _ => new CommandHandlerWrapper(handler)));
            return this;
        }
    }
}