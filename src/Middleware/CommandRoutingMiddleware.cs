using System;
using System.Threading;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Middleware
{
    internal class CommandRoutingMiddleware : IPipelineMiddleware<RequestContext>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICommandRouter _commandRouter;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="serviceProvider">Service provider</param>
        /// <param name="commandRouter">Command router</param>
        public CommandRoutingMiddleware(IServiceProvider serviceProvider, ICommandRouter commandRouter)
        {
            _serviceProvider = serviceProvider;
            _commandRouter = commandRouter;
        }
        
        /// <inheritdoc />
        public Task InvokeAsync(RequestContext context, 
            PipelineDelegate<RequestContext> next, 
            CancellationToken cancellationToken)
        {
            return _commandRouter.RouteAsync(_serviceProvider, context, cancellationToken);
        }
    }
}