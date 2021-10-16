using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Utilities;

namespace Vertical.ConsoleApplications.Providers
{
    internal class InteractiveArgumentsProvider : IArgumentsProvider
    {
        private readonly ILogger<InteractiveArgumentsProvider>? _logger;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="logger">Logger</param>
        public InteractiveArgumentsProvider(ILogger<InteractiveArgumentsProvider>? logger = null)
        {
            _logger = logger;
        }
        
        /// <inheritdoc />
        public async Task InvokeArgumentsAsync(Func<IReadOnlyList<string>, CancellationToken, Task> handler, 
            CancellationToken cancellationToken)
        {
            _logger.LogTrace("Entering interactive console provider");
            
            while (!cancellationToken.IsCancellationRequested)
            {
                var input = Console.ReadLine();

                var args = Arguments.SplitFromString(input ?? string.Empty);

                await handler(args, cancellationToken);
            }
        }
    }
}