using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Routing
{
    /// <summary>
    /// Represents a service that perform per-command routing.
    /// </summary>
    public interface ICommandRouter
    {
        /// <summary>
        /// Routes the command.
        /// </summary>
        /// <param name="args">Arguments that determines the routing.</param>
        /// <param name="cancellationToken">
        /// A token that can be observed for cancellation requests.
        /// </param>
        /// <returns>Task that completes when the command was handled.</returns>
        Task RouteAsync(IReadOnlyList<string> args, CancellationToken cancellationToken);
    }
}