using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Pipeline;

namespace Vertical.ConsoleApplications.Middleware
{
    internal class ExitCommandMiddleware : IMiddleware
    {
        private readonly string[] _commands;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILogger<ExitCommandMiddleware>? _logger;

        internal ExitCommandMiddleware(string[] commands,
            IHostApplicationLifetime applicationLifetime,
            ILogger<ExitCommandMiddleware>? logger = null)
        {
            _commands = commands;
            _applicationLifetime = applicationLifetime;
            _logger = logger;
        }
        
        /// <inheritdoc />
        public Task InvokeAsync(CommandContext context, PipelineDelegate next, CancellationToken cancellationToken)
        {
            if (_commands.Any(cmd => cmd == context.OriginalFormat))
            {
                _logger?.LogDebug("Exit command matched, requesting application stop");
                
                _applicationLifetime.StopApplication();
            }

            return next(context, cancellationToken);
        }
    }
}