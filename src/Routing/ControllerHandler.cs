using System;
using System.Threading;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Pipeline;

namespace Vertical.ConsoleApplications.Routing
{
    public class ControllerHandler<T> : ICommandHandler
    {
        
        /// <inheritdoc />
        public Task HandleAsync(CommandContext context, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}