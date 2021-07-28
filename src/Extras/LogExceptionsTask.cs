using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Extras
{
    /// <summary>
    /// Represents a pipeline task that logs exceptions in the pipeline.
    /// </summary>
    public class LogExceptionsTask : IPipelineTask<CommandContext>
    {
        private readonly ILogger<LogExceptionsTask> logger;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="logger">Logger</param>
        public LogExceptionsTask(ILogger<LogExceptionsTask> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task InvokeAsync(CommandContext context, 
            PipelineDelegate<CommandContext> next, 
            CancellationToken cancellationToken)
        {
            try
            {
                await next.InvokeAsync(context, cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception.Message);

                if (logger.IsEnabled(LogLevel.Trace))
                {
                    logger.LogTrace(exception.GetType().ToString());
                    logger.LogTrace(exception.StackTrace);
                }
            }
        }
    }
}