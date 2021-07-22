using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Providers
{
    /// <summary>
    /// Represents a provider that invokes commands received from user input.
    /// </summary>
    public class InteractiveConsoleProvider : ICommandProvider
    {
        private readonly Action prompt;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="prompt">An action that is executed before accepting user input.</param>
        public InteractiveConsoleProvider(Action? prompt = null)
        {
            this.prompt = prompt ?? new Action(() => Console.Write("> "));
        }
        
        /// <inheritdoc />
        public async Task ExecuteCommandsAsync(Func<string[], Task> asyncInvoke, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                prompt();

                var input = Console.ReadLine();
                
                
            }
        }
    }
}