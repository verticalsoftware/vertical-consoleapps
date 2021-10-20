using System;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Pipeline;

namespace Vertical.ConsoleApplications.Routing
{
    /// <summary>
    /// Represents a service that routes commands to a mapped handler.
    /// </summary>
    public interface ICommandRouter
    {
        /// <summary>
        /// Performs command routing.
        /// </summary>
        /// <param name="serviceProvider">Service provider for the current pipeline scope</param>
        /// <param name="context">Arguments context</param>
        /// <returns>Task that completes when the service completes routing.</returns>
        Task RouteAsync(IServiceProvider serviceProvider, ArgumentsContext context);
    }
}