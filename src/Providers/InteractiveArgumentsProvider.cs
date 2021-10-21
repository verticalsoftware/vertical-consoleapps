using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.IO;
using Vertical.ConsoleApplications.Utilities;

namespace Vertical.ConsoleApplications.Providers
{
    internal class InteractiveArgumentsProvider : IArgumentsProvider
    {
        private readonly IConsoleInputAdapter _consoleInputAdapter;
        private readonly Action _prompt;
        private readonly ILogger<InteractiveArgumentsProvider>? _logger;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="prompt">The prompt to display before allowing using input</param>
        /// <param name="logger">Logger</param>
        /// <param name="consoleInputAdapter">Console adapter</param>
        public InteractiveArgumentsProvider(
            IConsoleInputAdapter consoleInputAdapter,
            Action prompt,
            ILogger<InteractiveArgumentsProvider>? logger = null)
        {
            _consoleInputAdapter = consoleInputAdapter;
            _prompt = prompt;
            _logger = logger;
        }
        
        /// <inheritdoc />
        public async Task InvokeArgumentsAsync(
            Func<string[], Task> handler, 
            CancellationToken cancellationToken)
        {
            _logger.LogTrace("Entering interactive console provider");
            
            while (!cancellationToken.IsCancellationRequested)
            {
                // Flush off-thread loggers
                await Task.Delay(250, CancellationToken.None);

                _prompt();

                var input = _consoleInputAdapter.ReadLine();

                var args = Arguments.SplitFromString(input ?? string.Empty);

                await handler(args);
            }
        }
    }
}