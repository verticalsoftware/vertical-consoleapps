using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Middleware
{
    /// <summary>
    /// Middleware component that facilitates the handling of commands
    /// using delegate callback.
    /// </summary>
    internal class CommandDelegateMiddleware
    {
        private readonly PipelineDelegate<ArgumentsContext> _next;
        private readonly IDictionary<string, Func<string[], CancellationToken, Task>> _handlerMap;
        private readonly ILogger<CommandDelegateMiddleware>? _logger;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="next">The next middleware function</param>
        /// <param name="handlerMap">A dictionary of command names to handler delegates</param>
        /// <param name="logger">Logger</param>
        public CommandDelegateMiddleware(PipelineDelegate<ArgumentsContext> next,
            IDictionary<string, Func<string[], CancellationToken, Task>> handlerMap,
            ILogger<CommandDelegateMiddleware>? logger = null)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _handlerMap = handlerMap ?? throw new ArgumentNullException(nameof(handlerMap));
            _logger = logger;
        }

        /// <summary>
        /// Invokes a mapped delegate for a command.
        /// </summary>
        /// <param name="context">Argument context</param>
        /// <returns>Task</returns>
        public async Task InvokeAsync(ArgumentsContext context)
        {
            var args = context.Arguments;

            if (args.Count == 0)
            {
                _logger?.LogDebug("Cannot determine handler (no command/argument count = 0)");
                return;
            }

            var command = args[0];

            if (_handlerMap.TryGetValue(command, out var handler))
            {
                _logger.LogInformation("Command '{command}' handled using mapping delegate", command);
                
                await handler(args.Skip(1).ToArray(), context.CancellationToken);
            }
            else
            {
                _logger.LogDebug("No mapping delegate found for command '{command}'", command);
            }

            await _next(context);
        }
    }
}