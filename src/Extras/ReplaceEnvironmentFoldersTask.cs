using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Extensions;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Extras
{
    public class ReplaceEnvironmentFoldersTask : IPipelineTask<CommandContext>
    {
        private readonly ILogger<ReplaceEnvironmentFoldersTask>? logger;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="logger">Logger</param>
        public ReplaceEnvironmentFoldersTask(ILogger<ReplaceEnvironmentFoldersTask>? logger = null)
        {
            this.logger = logger;
        }
        
        /// <inheritdoc />
        public Task InvokeAsync(CommandContext context, 
            PipelineDelegate<CommandContext> next, 
            CancellationToken cancellationToken)
        {
            var replacedArguments = context
                .Arguments
                .Select(ReplaceWithEnvironmentFolders)
                .ToArray();

            return next.InvokeAsync(new CommandContext(replacedArguments), cancellationToken);
        }

        public string ReplaceWithEnvironmentFolders(string argument)
        {
            logger?.LogTrace("Evaluating arguments for environment folder symbols");

            return argument.ReplaceSymbols(symbol =>
            {
                if (!Enum.TryParse(symbol, true, out Environment.SpecialFolder specialFolder))
                {
                    logger?.LogTrace("Symbol '{symbol}' not found in special folders", symbol);
                }

                var path = Environment.GetFolderPath(specialFolder);

                logger.LogTrace("Replacing symbol in argument with environment special folder"
                + Environment.NewLine
                + "  Symbol : {symbol}"
                + Environment.NewLine
                + "  Path   : {value}",
                    symbol,
                    path);

                return path;
            }) ?? argument;
        }
    }
}