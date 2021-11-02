using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Pipeline
{
    internal sealed class MiddlewareWrapper : IMiddleware 
    {
        private readonly Func<RequestContext, PipelineDelegate, CancellationToken, Task> _middleware;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="middleware">Handler</param>
        internal MiddlewareWrapper(Func<RequestContext, PipelineDelegate, CancellationToken, Task> middleware)
        {
            _middleware = middleware;
        }

        /// <inheritdoc />
        public Task InvokeAsync(RequestContext context, PipelineDelegate next, CancellationToken cancellationToken)
        {
            return _middleware(context, next, cancellationToken);
        }
    }
}