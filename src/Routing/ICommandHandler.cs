using System.Threading;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Pipeline;

namespace Vertical.ConsoleApplications.Routing
{
    /// <summary>
    /// Represents an object that handles command requests for a specific argument
    /// set.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// When implemented by a class, handles the routed command request.
        /// </summary>
        /// <param name="context">Command context</param>
        /// <param name="cancellationToken">Token that can be observed for cancellation requests</param>
        /// <returns>Task</returns>
        Task HandleAsync(RequestContext context, CancellationToken cancellationToken);
    }
}