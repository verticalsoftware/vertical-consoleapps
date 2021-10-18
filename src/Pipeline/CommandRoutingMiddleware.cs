using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Routing;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Pipeline
{
    internal class CommandRoutingMiddleware
    {
        private readonly PipelineDelegate<ArgumentsContext> _next;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="next">The next delegate handler</param>
        public CommandRoutingMiddleware(PipelineDelegate<ArgumentsContext> next)
        {
            _next = next;
        }

        /// <summary>
        /// Handles the arguments in the pipeline
        /// </summary>
        /// <param name="context">Context that encapsulates the arguments</param>
        /// <param name="router">Routing component</param>
        /// <returns>Task</returns>
        public Task InvokeAsync(ArgumentsContext context, ICommandRouter router)
        {
            return router.RouteAsync(context.Arguments, context.CancellationToken);
        }
    }
}