using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Vertical.ConsoleApplications.Providers
{
    /// <summary>
    /// Implements <see cref="IArgumentsProvider"/> using known values.
    /// </summary>
    internal class StaticArgumentsProvider : IArgumentsProvider
    {
        private readonly string[] _arguments;
        private readonly string _context;
        private readonly ILogger<StaticArgumentsProvider>? _logger;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <param name="context">A context to attach to diagnostic events.</param>
        /// <param name="logger">Logger instance</param>
        /// <exception cref="ArgumentNullException"><paramref name="arguments"/> is null.</exception>
        public StaticArgumentsProvider(
            string[] arguments,
            string context,
            ILogger<StaticArgumentsProvider>? logger = default)
        {
            _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            _context = context;
            _logger = logger;
        }

        /// <inheritdoc />
        public override string ToString() => string.Join(" ", _arguments);

        /// <inheritdoc />
        public Task InvokeArgumentsAsync(
            Func<string[], Task> handler, 
            CancellationToken cancellationToken)
        {
            _logger?.LogTrace("Invoke {context} arguments ({count}): {values}", 
                _context,
                _arguments.Length, 
                _arguments);

            return handler(_arguments);
        }
    }
}