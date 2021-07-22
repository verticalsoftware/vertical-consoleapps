using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Vertical.ConsoleApplications.Providers
{
    public class StaticArgumentsProvider : ICommandProvider
    {
        private readonly string[] arguments;

        /// <summary>
        /// Creates a new instance 
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="arguments">Arguments to send to the command pipeline</param>
        public StaticArgumentsProvider(string[] arguments)
        {
            this.arguments = arguments;
        }
        
        /// <inheritdoc />
        public Task ExecuteCommandsAsync(Func<string[], Task> asyncInvoke, CancellationToken cancellationToken)
        {
            return asyncInvoke(arguments);
        }
    }
}