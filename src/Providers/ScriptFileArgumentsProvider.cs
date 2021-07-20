using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Logging;

namespace Vertical.ConsoleApplications.Providers
{
    /// <summary>
    /// Provides arguments by reading lines of a script file.
    /// </summary>
    public class ScriptFileArgumentsProvider : IArgumentProvider
    {
        private readonly ILogger logger = LogManager.CreateLogger<ScriptFileArgumentsProvider>();
        private readonly string matchPattern;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="matchPattern">Glob pattern</param>
        public ScriptFileArgumentsProvider(string matchPattern)
        {
            this.matchPattern = matchPattern;
        }
        
        /// <inheritdoc />
        public Task ExecuteInvocationsAsync(Func<string[], Task> invoke)
        {
            throw new NotImplementedException();
        }
    }
}