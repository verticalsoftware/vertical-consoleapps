using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Pipeline;

namespace Vertical.ConsoleApplications.Routing
{
    internal class CommandRouter : ICommandRouter
    {
        private readonly Dictionary<string, Func<IServiceProvider, ICommandHandler>> _handlerFactories;
        private readonly ILogger<CommandRouter>? _logger;

        /// <summary>
        /// Creates a new service of this type.
        /// </summary>
        /// <param name="handlerFactories">
        /// A dictionary of factory methods that produce handler instances where the
        /// key is a command.
        /// </param>
        /// <param name="logger">Logger</param>
        public CommandRouter(Dictionary<string, Func<IServiceProvider, ICommandHandler>> handlerFactories,
            ILogger<CommandRouter>? logger)
        {
            _handlerFactories = handlerFactories;
            _logger = logger;
        }

        /// <inheritdoc />
        public Task RouteAsync(IServiceProvider serviceProvider, 
            ArgumentsContext context)
        {
            var arguments = context.Arguments;
            
            if (arguments.Count == 0)
            {
                _logger?.LogTrace("Command routing unavailable (argument count =0)");
                return Task.CompletedTask;
            }

            var command = arguments[0];

            if (!_handlerFactories.TryGetValue(command, out var factory))
            {
                _logger?.LogDebug("No handler mapped for command '{command}'", command);
                return Task.CompletedTask;
            }

            var handler = factory(serviceProvider);
            
            _logger?.LogDebug("Routing command '{command}' to handler {handler}",
                command,
                handler);

            return handler.HandleAsync(arguments.Skip(1).ToArray(), context.CancellationToken);
        }
    }
}