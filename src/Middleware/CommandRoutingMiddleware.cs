using System.Threading;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Middleware
{
    internal class CommandRoutingMiddleware : IPipelineMiddleware<RequestContext>
    {
        private readonly ICommandRouter _commandRouter;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="commandRouter">Command router</param>
        public CommandRoutingMiddleware(ICommandRouter commandRouter)
        {
            _commandRouter = commandRouter;
        }
        
        /// <inheritdoc />
        public async Task InvokeAsync(RequestContext context, 
            PipelineDelegate<RequestContext> next, 
            CancellationToken cancellationToken)
        {
            await _commandRouter.RouteAsync(context, cancellationToken);
            await next(context, cancellationToken);
        }
    }
}