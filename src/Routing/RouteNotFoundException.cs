using System;
using System.Linq;

namespace Vertical.ConsoleApplications.Routing
{
    public class RouteNotFoundException : Exception
    {
        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="arguments">Arguments that failed to route</param>
        public RouteNotFoundException(string[] arguments) : base(BuildMessage(arguments))
        {
        }

        private static string BuildMessage(string[] arguments)
        {
            if (arguments.Length == 0)
                return "(empty)";

            var pathCommands = arguments
                .TakeWhile(arg => !arg.StartsWith('-') && !arg.StartsWith('/'))
                .ToArray();

            var path = pathCommands.Length == 0
                ? string.Join(' ', arguments)
                : string.Join(' ', pathCommands) + $" (+{arguments.Length - pathCommands.Length} likely options)";

            return $"No handler implementation for path \"{path}\" found";
        }
    }
}