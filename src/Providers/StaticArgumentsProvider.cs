using System;
using System.Collections.Generic;
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
        private readonly IReadOnlyList<string> _arguments;
        private readonly ILogger<StaticArgumentsProvider>? _logger;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <param name="logger">Logger instance</param>
        /// <exception cref="ArgumentNullException"><paramref name="arguments"/> is null.</exception>
        public StaticArgumentsProvider(
            IReadOnlyList<string> arguments,
            ILogger<StaticArgumentsProvider>? logger = default)
        {
            _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            _logger = logger;
        }

        /// <inheritdoc />
        public override string ToString() => string.Join(" ", _arguments);

        /// <inheritdoc />
        public Task InvokeArgumentsAsync(
            Func<IReadOnlyList<string>, CancellationToken, Task> handler, 
            CancellationToken cancellationToken)
        {
            _logger?.LogTrace("Invoke static arguments ({count}): {values}", _arguments.Count, _arguments);

            return handler(_arguments, cancellationToken);
        }
    }
}