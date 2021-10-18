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
        private readonly IConsoleAdapter _consoleAdapter;
        private readonly string _prompt;
        private readonly ILogger<InteractiveArgumentsProvider>? _logger;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="prompt">The prompt to display before allowing using input</param>
        /// <param name="logger">Logger</param>
        /// <param name="consoleAdapter">Console adapter</param>
        public InteractiveArgumentsProvider(
            IConsoleAdapter consoleAdapter,
            string prompt = "",
            ILogger<InteractiveArgumentsProvider>? logger = null)
        {
            _consoleAdapter = consoleAdapter;
            _prompt = prompt;
            _logger = logger;
        }
        
        /// <inheritdoc />
        public async Task InvokeArgumentsAsync(
            Func<IReadOnlyList<string>, CancellationToken, Task> handler, 
            CancellationToken cancellationToken)
        {
            _logger.LogTrace("Entering interactive console provider");
            
            while (!cancellationToken.IsCancellationRequested)
            {
                // Flush off-thread loggers
                await Task.Delay(250, CancellationToken.None);
                
                _consoleAdapter.Write(_prompt);

                var input = _consoleAdapter.ReadLine();

                var args = Arguments.SplitFromString(input ?? string.Empty);

                await handler(args, cancellationToken);
            }
        }
    }
}