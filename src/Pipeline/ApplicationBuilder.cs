using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Extras;
using Vertical.ConsoleApplications.Routing;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Pipeline
{
    /// <summary>
    /// Object used to build the application pipeline.
    /// </summary>
    public class ApplicationBuilder
    {
        private readonly IServiceCollection services;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public ApplicationBuilder(IServiceCollection services)
        {
            this.services = services;
            
            services.AddSingleton<IPipelineOrchestrator, PipelineOrchestrator>();
        }

        /// <summary>
        /// Uses an instance of a pipeline task.
        /// </summary>
        /// <param name="taskInstance">Task instance.</param>
        /// <returns>A reference to this instance.</returns>
        public ApplicationBuilder UseTask(IPipelineTask<CommandContext> taskInstance)
        {
            services.AddSingleton(taskInstance);
            return this;
        }

        /// <summary>
        /// Registers a type of pipeline task.
        /// </summary>
        /// <typeparam name="T">Task implementation type.</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ApplicationBuilder UseTask<T>() where T : class, IPipelineTask<CommandContext>
        {
            services.AddSingleton<IPipelineTask<CommandContext>, T>();
            return this;
        }

        /// <summary>
        /// Registers a pipeline task factory.
        /// </summary>
        /// <param name="factory">Factory function used to created the service instance.</param>
        /// <typeparam name="T">Task implementation type.</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ApplicationBuilder UseTask<T>(Func<IServiceProvider, T> factory) 
            where T : class, IPipelineTask<CommandContext>
        {
            services.AddSingleton<IPipelineTask<CommandContext>>(factory);
            return this;
        }

        /// <summary>
        /// Registers a pipeline task whose logic is encapsulated in a delegate.
        /// </summary>
        /// <param name="implementation">Implementation delegate.</param>
        /// <returns>A reference to this instance.</returns>
        public ApplicationBuilder Use(Func<CommandContext, PipelineDelegate<CommandContext>, CancellationToken, Task>
            implementation)
        {
            services.AddSingleton<IPipelineTask<CommandContext>>(new WrappedPipelineTask(implementation));
            return this;
        }

        /// <summary>
        /// Adds the pipeline task necessary to route commands to controller handlers.
        /// </summary>
        /// <returns>A reference to this instance.</returns>
        public ApplicationBuilder UseCommandRouting()
        {
            services.AddSingleton<IPipelineTask<CommandContext>, RouteCommandTask>();
            return this;
        }

        /// <summary>
        /// Adds the pipeline task that catches and logs exceptions thrown in the pipeline.
        /// </summary>
        /// <returns>A reference to this instance.</returns>
        public ApplicationBuilder UseExceptionLogging()
        {
            services.AddSingleton<IPipelineTask<CommandContext>, LogExceptionsTask>();
            return this;
        }
        
        /// <summary>
        /// Adds a pipeline task that catches a specific type of exception and invokes a callback.
        /// </summary>
        /// <param name="callback">A callback function that returns whether the exception was handled.</param>
        /// <typeparam name="T">Exception type.</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ApplicationBuilder UseExceptionHandler<T>(Action callback)
            where T : Exception
        {
            return UseExceptionHandler<T>((_, __) =>
            {
                callback();
                return true;
            });
        }

        /// <summary>
        /// Adds a pipeline task that catches a specific type of exception and invokes a callback.
        /// </summary>
        /// <param name="callback">A callback function that returns whether the exception was handled.</param>
        /// <typeparam name="T">Exception type.</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ApplicationBuilder UseExceptionHandler<T>(Func<CommandContext, T, bool> callback)
            where T : Exception
        {
            services.AddSingleton<IPipelineTask<CommandContext>>(provider => new HandleExceptionTask<T>(
                callback, provider.GetService<ILogger<HandleExceptionTask<T>>>()));
            return this;
        }

        /// <summary>
        /// Adds the pipeline task that transforms arguments with symbols to matching
        /// environment variables.
        /// </summary>
        /// <returns>A reference to this instance.</returns>
        public ApplicationBuilder UseEnvironmentVariableReplacements()
        {
            services.AddSingleton<IPipelineTask<CommandContext>, ReplaceEnvironmentVariablesTask>();
            return this;
        }
        
        /// <summary>
        /// Adds the pipeline task that transforms arguments with symbols to matching
        /// environment folder paths.
        /// </summary>
        /// <returns>A reference to this instance.</returns>
        public ApplicationBuilder UseEnvironmentFolderReplacements()
        {
            services.AddSingleton<IPipelineTask<CommandContext>, ReplaceEnvironmentFoldersTask>();
            return this;
        }
    }
}