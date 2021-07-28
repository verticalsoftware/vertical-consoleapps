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
    /// <summary>
    /// Replaces symbols in the arguments with environment variable values.
    /// </summary>
    public class ReplaceEnvironmentVariablesTask : IPipelineTask<CommandContext>
    {
        private readonly ILogger<ReplaceEnvironmentVariablesTask>? logger;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="logger">Logger</param>
        public ReplaceEnvironmentVariablesTask(ILogger<ReplaceEnvironmentVariablesTask>? logger)
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
                .Select(ReplaceWithEnvironmentVariables)
                .ToArray();

            return next.InvokeAsync(new CommandContext(replacedArguments), cancellationToken);
        }

        private string ReplaceWithEnvironmentVariables(string argument)
        {
            logger?.LogTrace("Evaluating arguments for environment variable symbols");
            
            return argument.ReplaceSymbols(symbol =>
            {
                var value = Environment.GetEnvironmentVariable(symbol);

                if (value != null)
                {
                    logger?.LogTrace("Replacing symbol in argument with environment variable value"
                                    + Environment.NewLine
                                    + "  Symbol : {symbol}"
                                    + Environment.NewLine
                                    + "  Path   : {value}",
                        symbol,
                        value);
                }
                else
                {
                    logger?.LogTrace("Symbol '{symbol}' not found in environment variables", symbol);
                }

                return value;
            }) ?? argument;
        }
    }
}