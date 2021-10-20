using System;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Middleware
{
    /// <summary>
    /// Middleware that passes control to routing defined in application services.
    /// </summary>
    public class CommandRoutingMiddleware
    {
        private readonly PipelineDelegate<ArgumentsContext> _next;
        private readonly ICommandRouter _commandRouter;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="next">A delegate to the next middleware.</param>
        /// <param name="commandRouter">Command routing service</param>
        public CommandRoutingMiddleware(PipelineDelegate<ArgumentsContext> next,
            ICommandRouter commandRouter)
        {
            _next = next;
            _commandRouter = commandRouter;
        }

        /// <summary>
        /// Invokes the router with the context arguments.
        /// </summary>
        /// <param name="context">Arguments context</param>
        /// <param name="serviceProvider">Scoped service provider</param>
        public async Task InvokeAsync(ArgumentsContext context, IServiceProvider serviceProvider)
        {
            await _commandRouter.RouteAsync(serviceProvider, context);
            
            await _next(context);
        }
    }
}