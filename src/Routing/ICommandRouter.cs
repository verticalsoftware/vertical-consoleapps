using System;
using System.Threading;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Pipeline;

namespace Vertical.ConsoleApplications.Routing
{
    /// <summary>
    /// Represents a service that routes action based on command arguments.
    /// </summary>
    public interface ICommandRouter
    {
        /// <summary>
        /// When implemented by a class, routes action based on command arguments to
        /// a pre-defined handler.
        /// </summary>
        /// <param name="serviceProvider">Command request service provider</param>
        /// <param name="context">Current command context</param>
        /// <param name="cancellationToken">Token that can be observed for cancellation requests</param>
        /// <returns>Task</returns>
        Task RouteAsync(IServiceProvider serviceProvider, 
            CommandContext context, 
            CancellationToken cancellationToken);
    }
}