using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Pipeline
{
    internal sealed class MiddlewareWrapper : IMiddleware 
    {
        private readonly Func<CommandContext, PipelineDelegate, CancellationToken, Task> _middleware;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="middleware">Handler</param>
        internal MiddlewareWrapper(Func<CommandContext, PipelineDelegate, CancellationToken, Task> middleware)
        {
            _middleware = middleware;
        }

        /// <inheritdoc />
        public Task InvokeAsync(CommandContext context, PipelineDelegate next, CancellationToken cancellationToken)
        {
            return _middleware(context, next, cancellationToken);
        }
    }
}