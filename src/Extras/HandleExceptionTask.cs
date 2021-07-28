using System;
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
                logger?.LogTrace("Caught exception {type}", exception.GetType());
                
                if (exception is T casted && callback(context, casted))
                    return;

                logger?.LogTrace("Exception callback return {false}, throwing", false);
                throw;
            }
        }
    }
}