using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Logging;

namespace Vertical.ConsoleApplications.Providers
{
    /// <summary>
    /// An arguments provider
    /// </summary>
    public class StaticArgumentsProvider : IArgumentProvider
    {
        private readonly ILogger logger = LogManager.CreateLogger<StaticArgumentsProvider>();
        private readonly string[] arguments;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="arguments">Arguments to execute.</param>
        public StaticArgumentsProvider(string[] arguments)
        {
            this.arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }
        
        /// <inheritdoc />
        public Task ExecuteInvocationsAsync(Func<string[], Task> invoke)
        {
            return invoke(arguments);
        }
    }
}