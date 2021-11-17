using System;

namespace Vertical.ConsoleApplications.Routing
{
    public class RouteDescriptor
    {
        /// <summary>
        /// Represents the registration of a route with a command handler implementation.
        /// </summary>
        /// <param name="route">The route handled by the command.</param>
        /// <param name="implementationFactory">A factory method that creates the instance.</param>
        public RouteDescriptor(string route, Func<IServiceProvider, ICommandHandler> implementationFactory)
        {
            Route = route;
            ImplementationFactory = implementationFactory;
        }
        
        /// <summary>
        /// Gets the route.
        /// </summary>
        public string Route { get; }
        
        /// <summary>
        /// Gets the implementation factory.
        /// </summary>
        public Func<IServiceProvider, ICommandHandler> ImplementationFactory { get; }

        /// <summary>
        /// Provides deconstruction for this type.
        /// </summary>
        /// <param name="route">Assigns the route</param>
        /// <param name="implementationFactory">Assigns the implementation factory</param>
        public void Deconstruct(out string route, out Func<IServiceProvider, ICommandHandler> implementationFactory)
        {
            route = Route;
            implementationFactory = ImplementationFactory;
        }

        /// <inheritdoc />
        public override string ToString() => Route;
    }
}