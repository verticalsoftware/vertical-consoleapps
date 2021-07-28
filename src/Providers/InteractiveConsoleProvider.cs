using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Extensions;

namespace Vertical.ConsoleApplications.Providers
{
    /// <summary>
    /// Represents a provider that invokes commands received from user input.
    /// </summary>
    public class InteractiveConsoleProvider : ICommandProvider
    {
        private readonly ILogger<InteractiveConsoleProvider>? logger;
        private readonly Action prompt;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="prompt">An action that is executed before accepting user input.</param>
        public InteractiveConsoleProvider(ILogger<InteractiveConsoleProvider>? logger = null, Action? prompt = null)
        {
            this.logger = logger;
            this.prompt = prompt ?? new Action(() => Console.Write("> "));
        }
        
        /// <inheritdoc />
        public async Task ExecuteCommandsAsync(Func<string[], Task> asyncInvoke, CancellationToken cancellationToken)
        {
            logger?.LogTrace("Initializing console input provider");

            while (!cancellationToken.IsCancellationRequested)
            {
                await Console.Out.FlushAsync();
                
                prompt();

                var input = Console.ReadLine();
                
                // Null for SIGTERM (doesn't work for some terminals)
                if (input == null)
                    return;
                
                if (string.IsNullOrWhiteSpace(input))
                    continue;

                var arguments = input.GetEscapedArguments();

                if (true == logger?.IsEnabled(LogLevel.Trace))
                {
                    logger.LogTrace("Input received, split to {count} argument(s):" 
                                    + Environment.NewLine
                                    + string.Join(Environment.NewLine, arguments.Select((arg, index) => $"  {index}> \"{arg}\"")),
                        arguments.Length);
                }

                await asyncInvoke(arguments);
            }
        }
    }
}