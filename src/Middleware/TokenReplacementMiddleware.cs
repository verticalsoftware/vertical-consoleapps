using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Middleware
{
    internal class TokenReplacementMiddleware
    {
        private readonly PipelineDelegate<ArgumentsContext> _next;
        private readonly string _context;
        private readonly Func<string, string> _tokenFunction;
        private readonly ILogger<TokenReplacementMiddleware>? _logger;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="next">Next request delegate</param>
        /// <param name="context">Description of the replacement</param>
        /// <param name="tokenFunction">A function that performs the replacements</param>
        /// <param name="logger">Logger</param>
        public TokenReplacementMiddleware(PipelineDelegate<ArgumentsContext> next,
            string context,
            Func<string, string> tokenFunction,
            ILogger<TokenReplacementMiddleware>? logger = null)
        {
            _next = next;
            _context = context;
            _tokenFunction = tokenFunction;
            _logger = logger;
        }

        public Task InvokeAsync(ArgumentsContext context)
        {
            var args = context.Arguments;

            for (var c = 0; c < args.Count; c++)
            {
                var replacement = _tokenFunction(args[c]);

                if (args[c] == replacement) 
                    continue;
                
                _logger?.LogTrace("Replaced argument {arg}='{value}' ({context})",
                    args[c],
                    replacement,
                    _context);

                args[c] = replacement;
            }

            return _next(context);
        }
    }
}