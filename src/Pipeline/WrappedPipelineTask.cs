using System;
using System.Threading;
using System.Threading.Tasks;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Pipeline
{
    /// <summary>
    /// Represents an object that wraps a pipeline task.
    /// </summary>
    public class WrappedPipelineTask : IPipelineTask<CommandContext>
    {
        private readonly Func<CommandContext, PipelineDelegate<CommandContext>, CancellationToken, Task> implementation;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="implementation">Implementation type.</param>
        public WrappedPipelineTask(Func<CommandContext, PipelineDelegate<CommandContext>, CancellationToken, Task>
            implementation)
        {
            this.implementation = implementation;
        }
        
        /// <inheritdoc />
        public Task InvokeAsync(CommandContext context, 
            PipelineDelegate<CommandContext> next, 
            CancellationToken cancellationToken)
        {
            return implementation(context, next, cancellationToken);
        }
    }
}