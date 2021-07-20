using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Pipeline
{
    /// <summary>
    /// Object used to construct the application pipeline.
    /// </summary>
    public class ApplicationBuilder
    {
        private readonly IServiceCollection services;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="services">Service collection.</param>
        public ApplicationBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        /// <summary>
        /// Inserts a task instance factory into the pipeline.
        /// </summary>
        /// <param name="factory">Factory function that creates the instance.</param>
        /// <typeparam name="T">Pipeline task type.</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ApplicationBuilder UsePipelineTask<T>(Func<IServiceProvider, T> factory) where T : class, IPipelineTask<string[]>
        {
            services.AddSingleton<IPipelineTask<string[]>>(factory);
            return this;
        }

        /// <summary>
        /// Inserts a task instance type registration into the pipeline.
        /// </summary>
        /// <typeparam name="T">Pipeline task type.</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ApplicationBuilder UsePipelineTask<T>() where T : class, IPipelineTask<string[]>
        {
            services.AddSingleton<IPipelineTask<string[]>, T>();
            return this;
        }

        /// <summary>
        /// Inserts a task instance into the pipeline.
        /// </summary>
        /// <param name="pipelineTask">The pipeline task.</param>
        /// <returns>A reference to this instance.</returns>
        public ApplicationBuilder UsePipelineTask(IPipelineTask<string[]> pipelineTask)
        {
            services.AddSingleton(pipelineTask);
            return this;
        }

        /// <summary>
        /// Inserts a delegate based handler into the pipeline.
        /// </summary>
        /// <param name="asyncHandler">An asynchronous function that handles the task logic.</param>
        /// <returns>A reference to this instance.</returns>
        public ApplicationBuilder Use(Func<string[], PipelineDelegate<string[]>, CancellationToken, Task> asyncHandler) => 
            UsePipelineTask(new PipelineTaskWrapper(asyncHandler));
    }
}