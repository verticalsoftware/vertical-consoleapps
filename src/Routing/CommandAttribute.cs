using System;

namespace Vertical.ConsoleApplications.Routing
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="route">The route for the handler.</param>
        public CommandAttribute(string route)
        {
            Route = route ?? throw new ArgumentNullException(nameof(route));

            if (string.IsNullOrWhiteSpace(route))
            {
                throw new ArgumentException("Route cannot be whitespace");
            }
        }

        /// <summary>
        /// Gets the command route.
        /// </summary>
        public string Route { get; }

        /// <inheritdoc />
        public override string ToString() => Route;
    }
}