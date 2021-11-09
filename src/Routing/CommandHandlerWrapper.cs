using System;
using System.Threading;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Pipeline;

namespace Vertical.ConsoleApplications.Routing
{
    /// <summary>
    /// Wraps implementation of <see cref="ICommandHandler"/> with a delegate.
    /// </summary>
    internal class CommandHandlerWrapper : ICommandHandler
    {
        private readonly Func<RequestContext, CancellationToken, Task> _handlerImplementation;

        internal CommandHandlerWrapper(Func<RequestContext, CancellationToken, Task> handlerImplementation)
        {
            _handlerImplementation = handlerImplementation 
                                     ?? 
                                     throw new ArgumentNullException(nameof(handlerImplementation));
        }
        
        /// <inheritdoc />
        public Task HandleAsync(RequestContext context, CancellationToken cancellationToken)
        {
            return _handlerImplementation(context, cancellationToken);
        }
    }
}