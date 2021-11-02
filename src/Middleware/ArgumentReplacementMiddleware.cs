using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Pipeline;

namespace Vertical.ConsoleApplications.Middleware
{
    internal class ArgumentReplacementMiddleware : IMiddleware
    {
        private readonly Func<string, string> _replaceFunction;
        private readonly ILogger<ArgumentReplacementMiddleware>? _logger;

        internal ArgumentReplacementMiddleware(Func<string, string> replaceFunction,
            ILogger<ArgumentReplacementMiddleware>? logger = null)
        {
            _replaceFunction = replaceFunction ?? throw new ArgumentNullException(nameof(replaceFunction));
            _logger = logger;
        }
        
        /// <inheritdoc />
        public Task InvokeAsync(RequestContext context, 
            PipelineDelegate next, 
            CancellationToken cancellationToken)
        {
            var args = context.Arguments;

            for (var c = 0; c < args.Length; c++)
            {
                var currentValue = args[c];
                var replaceValue = _replaceFunction(currentValue);

                if (string.Compare(currentValue, replaceValue, StringComparison.Ordinal) == 0) 
                    continue;
                
                _logger?.LogTrace(
                    "Token replacement, '{oldValue}'='{newValue}'",
                    currentValue,
                    replaceValue);

                args[c] = replaceValue;
            }

            return next(context, cancellationToken);
        }
    }
}