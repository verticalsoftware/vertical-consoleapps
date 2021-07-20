using System;
using System.Collections.Generic;

namespace Vertical.ConsoleApplications.Commands
{
    /// <summary>
    /// Defines options for command routing.
    /// </summary>
    public class CommandRouteOptions
    {
        /// <summary>
        /// Gets a HashSet of types that have command handlers methods.
        /// </summary>
        public HashSet<Type> RouteControllerTypes { get; } = new();
    }
}