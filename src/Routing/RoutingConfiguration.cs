using System.Collections.Generic;

namespace Vertical.ConsoleApplications.Routing
{
    /// <summary>
    /// Defines the routing configuration.
    /// </summary>
    public class RoutingConfiguration
    {
        /// <summary>
        /// Gets the map of routes to invocation metadata.
        /// </summary>
        public Dictionary<string, RouteMetadata> RouteMap { get; } = new();
    }
}