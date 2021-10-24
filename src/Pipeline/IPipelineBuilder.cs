using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Vertical.ConsoleApplications.Pipeline
{
    /// <summary>
    /// Represents the interface of an object used to construct middleware pipelines.
    /// </summary>
    public interface IPipelineBuilder
    {
        /// <summary>
        /// Registers a delegate as a middleware component.
        /// </summary>
        /// <param name="middleware">
        /// A delegate that implements the middleware
        /// </param>
        /// <returns>A reference to this instance</returns>
        PipelineBuilder Use(Func<CommandContext, PipelineDelegate, CancellationToken, Task> middleware);

        /// <summary>
        /// Registers a middleware component.
        /// </summary>
        /// <typeparam name="T">Middleware component type</typeparam>
        /// <returns>A reference to this instance</returns>
        PipelineBuilder UseMiddleware<T>() where T : class, IMiddleware;

        /// <summary>
        /// Registers creation of a middleware component.
        /// </summary>
        /// <param name="implementationFactory">A function that creates the middleware instance.</param>
        /// <typeparam name="T">Middleware component type</typeparam>
        /// <returns>A reference to this instance</returns>
        PipelineBuilder UseMiddleware<T>(Func<IServiceProvider, T> implementationFactory) where T : IMiddleware;

        /// <summary>
        /// Gets the application services.
        /// </summary>
        IServiceCollection ApplicationServices { get; }
    }
}