using System;
using System.Threading;
using System.Threading.Tasks;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Pipeline
{
    /// <summary>
    /// Encapsulates a pipeline task.
    /// </summary>
    public class PipelineTaskWrapper : IPipelineTask<string[]>
    {
        private readonly Func<string[], PipelineDelegate<string[]>, CancellationToken, Task> asyncHandler;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="asyncHandler">The implementation function.</param>
        public PipelineTaskWrapper(Func<string[], PipelineDelegate<string[]>, CancellationToken, Task> asyncHandler)
        {
            this.asyncHandler = asyncHandler;
        }
        
        /// <inheritdoc />
        public Task InvokeAsync(string[] context, 
            PipelineDelegate<string[]> next, 
            CancellationToken cancellationToken)
        {
            return asyncHandler(context, next, cancellationToken);
        }
    }
}