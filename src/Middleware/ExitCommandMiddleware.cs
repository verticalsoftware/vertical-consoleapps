using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Middleware
{
    internal class ExitCommandMiddleware
    {
        private readonly PipelineDelegate<ArgumentsContext> _next;
        private readonly string[] _exitCommands;
        private readonly ILogger<ExitCommandMiddleware>? _logger;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="next">The next delegate.</param>
        /// <param name="exitCommands">Exit commands.</param>
        /// <param name="logger">Logger</param>
        public ExitCommandMiddleware(
            PipelineDelegate<ArgumentsContext> next,
            string[] exitCommands,
            ILogger<ExitCommandMiddleware>? logger = null)
        {
            _next = next;
            _exitCommands = exitCommands;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">Arguments context</param>
        /// <returns>Task</returns>
        public async Task InvokeAsync(ArgumentsContext context)
        {
            var args = context.Arguments;

            if (args.Count == 1 && _exitCommands.Any(cmd => args[0] == cmd))
            {
                _logger.LogTrace("Exit command received - requesting application stop");

                context.StopApplication();

                return;
            }

            await _next(context);
        }
    }
}