using System;
using System.Threading;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Pipeline;

namespace Vertical.ConsoleApplications.Routing
{
    public class ControllerHandler<T> : ICommandHandler where T : class
    {
        private readonly T _controller;
        private readonly Func<T, CommandContext, CancellationToken, Task> _handler;

        public ControllerHandler(T controller, Func<T, CommandContext, CancellationToken, Task> handler)
        {
            _controller = controller;
            _handler = handler;
        }
        
        /// <inheritdoc />
        public Task HandleAsync(CommandContext context, CancellationToken cancellationToken)
        {
            return _handler(_controller, context, cancellationToken);
        }
    }
}