using System.Threading;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Routing
{
    /// <summary>
    /// Parses and dispatches arguments to command handlers.
    /// </summary>
    public interface ICommandRouter
    {
        /// <summary>
        /// Given the arguments, attempts to route action to a command.
        /// </summary>
        /// <param name="arguments">Command arguments</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Whether the command was routed</returns>
        Task<bool> TryRouteCommandAsync(string[] arguments, CancellationToken cancellationToken);
    }
}