using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Extras
{
    /// <summary>
    /// Handles specific exceptions that occur in the pipeline.
    /// </summary>
    /// <typeparam name="T">Exception type.</typeparam>
    public class HandleExceptionTask<T> : IPipelineTask<CommandContext> where T : Exception
    {
        private readonly Func<CommandContext, T, bool> callback;
        private readonly ILogger<HandleExceptionTask<T>>? logger;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="callback">Function that evaluates the arguments and the exception,
        /// and return whether or not the condition was handled.</param>
        /// <param name="logger">Logger.</param>
        public HandleExceptionTask(Func<CommandContext, T, bool> callback,
            ILogger<HandleExceptionTask<T>>? logger = null)
        {
            this.callback = callback;
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
                var handled = exception is T typeMatch && callback(context, typeMatch);

                if (true == logger?.IsEnabled(LogLevel.Trace))
                {
                    var firstFrame = exception.StackTrace?.Split(Environment.NewLine)?.FirstOrDefault();
                    
                    logger.LogTrace($"Exception {exception.GetType()} caught (first-chance)"
                                    + Environment.NewLine
                                    + $"  Stack:    {firstFrame?.Trim()}"
                                    + Environment.NewLine
                                    + $"  Watching: {typeof(T)}"
                                    + Environment.NewLine
                                    + $"  Handled:  {handled}");
                }

                if (!handled)
                {
                    throw;
                }
            }
        }
    }
}