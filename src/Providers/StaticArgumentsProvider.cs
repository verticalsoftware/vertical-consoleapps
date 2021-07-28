using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Vertical.ConsoleApplications.Providers
{
    public class StaticArgumentsProvider : ICommandProvider
    {
        private readonly ILogger<StaticArgumentsProvider>? logger;
        private readonly string[] arguments;

        /// <summary>
        /// Creates a new instance 
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="arguments">Arguments to send to the command pipeline</param>
        public StaticArgumentsProvider(string[] arguments, ILogger<StaticArgumentsProvider>? logger = null)
        {
            this.logger = logger;
            this.arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }
        
        /// <inheritdoc />
        public Task ExecuteCommandsAsync(Func<string[], Task> asyncInvoke, CancellationToken cancellationToken)
        {
            if (arguments.Length == 0)
            {
                logger?.LogTrace("Arguments array is empty, invocation will be ignored");
                return Task.CompletedTask;
            }
            
            logger?.LogTrace(
                "Evaluating {count} static arguments:"
                + Environment.NewLine
                + string.Join(Environment.NewLine, arguments.Select((arg, index) => $"  {index}> {arg}")),
                arguments.Length);
            
            return asyncInvoke(arguments);
        }
    }
}