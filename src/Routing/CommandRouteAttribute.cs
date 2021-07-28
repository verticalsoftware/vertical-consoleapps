using System;

namespace Vertical.ConsoleApplications.Routing
{
    /// <summary>
    /// Decorates a class or method with a route.
    /// </summary>
    public class CommandRouteAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="route">The route to take.</param>
        public CommandRouteAttribute(string route)
        {
            Route = route;
        }

        /// <summary>
        /// Gets the command route.
        /// </summary>
        public string Route { get; }

        /// <inheritdoc />
        public override string ToString() => Route;
    }
}