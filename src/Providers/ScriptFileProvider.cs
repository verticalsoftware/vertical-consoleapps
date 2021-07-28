using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Extensions;

namespace Vertical.ConsoleApplications.Providers
{
    /// <summary>
    /// A command provider that reads entries from a script file.
    /// </summary>
    public class ScriptFileProvider : ICommandProvider
    {
        private readonly string path;
        private readonly bool optional;
        private readonly ILogger<ScriptFileProvider>? logger;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="path">Path to the script to read.</param>
        /// <param name="optional">Whether the script is optional.</param>
        /// <param name="logger">Logger</param>
        public ScriptFileProvider(string path, 
            bool optional = false,
            ILogger<ScriptFileProvider>? logger = null)
        {
            this.path = path;
            this.optional = optional;
            this.logger = logger;
        }
        
        /// <inheritdoc />
        public async Task ExecuteCommandsAsync(Func<string[], Task> asyncInvoke, 
            CancellationToken cancellationToken)
        {
            try
            {
                logger?.LogTrace(
                    "Try reading command from script"
                    + Environment.NewLine
                    + "  Path     : {path}"
                    + Environment.NewLine
                    + "  Optional : {optional}",
                    path, optional);

                var content = await File.ReadAllLinesAsync(path, cancellationToken);

                if (content.Length == 0)
                {
                    logger.LogTrace("Script did not contain any content");
                }

                var lineNumber = 1;

                foreach (var line in content.Select(str => str.Trim()).Where(str => str.Length > 0))
                {
                    var arguments = line.GetEscapedArguments();

                    logger?.LogTrace("Evaluating script arguments (line {line}): [{arguments}]",
                        lineNumber,
                        string.Join(",", arguments.Select(arg => $"\"{arg}\"")));

                    await asyncInvoke(arguments);

                    lineNumber++;
                }
            }
            catch (DirectoryNotFoundException)
            {
                logger.LogTrace("Could not find file '{path}', optional = {optional}", path, optional);
                if (!optional) { throw; }
            }
            catch (FileNotFoundException)
            {
                logger.LogTrace("Could not find file '{path}', optional = {optional}", path, optional);
                if (!optional) { throw; }
            }
        }
    }
}