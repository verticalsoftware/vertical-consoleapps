using System;
using System.Threading;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Middleware
{
    internal class UnmatchedRouteMiddleware : IPipelineMiddleware<RequestContext>
    {
        private readonly Action<RequestContext> _action;

        internal UnmatchedRouteMiddleware(Action<RequestContext> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }
        
        /// <inheritdoc />
        public async Task InvokeAsync(RequestContext context, 
            PipelineDelegate<RequestContext> next, 
            CancellationToken cancellationToken)
        {
            await next(context, cancellationToken);

            if (context.Items.Get<RouteDescriptor>() != null)
            {
                return;
            }

            _action(context);
        }
    }
}