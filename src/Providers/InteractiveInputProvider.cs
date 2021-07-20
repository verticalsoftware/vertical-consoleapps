using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Logging;
using Vertical.ConsoleApplications.Parsing;

namespace Vertical.ConsoleApplications.Providers
{
    /// <summary>
    /// Represents a provider that listens for interactive user input.
    /// </summary>
    public class InteractiveInputProvider : IArgumentProvider
    {
        private readonly ILogger logger = LogManager.CreateLogger<InteractiveInputProvider>();
        private readonly Action prompt;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="prompt">Action that can be used to display a prompt.</param>
        public InteractiveInputProvider(Action? prompt = null)
        {
            this.prompt = prompt ?? new Action(() => Console.Write("> "));
        }
        
        /// <inheritdoc />
        public async Task ExecuteInvocationsAsync(Func<string[], Task> invoke)
        {
            for (;;)
            {
                prompt();

                var input = Console.ReadLine();
                var arguments = input?.SplitToArguments().ToArray() ?? Array.Empty<string>();

                logger.LogTrace("Received {count} characters from Console input ({num} arguments parsed)",
                    input?.Length, arguments.Length);
                
                await invoke(arguments);
            }
        }
    }
}