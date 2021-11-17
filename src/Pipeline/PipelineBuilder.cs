using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Vertical.ConsoleApplications.Pipeline
{
    public class PipelineBuilder : IPipelineBuilder
    {
        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="applicationServices">Application services</param>
        internal PipelineBuilder(IServiceCollection applicationServices)
        {
            ApplicationServices = applicationServices;

            applicationServices.AddScoped<IMiddlewareFactory, MiddlewareFactory>();
        }

        /// <summary>
        /// Gets the application services.
        /// </summary>
        public IServiceCollection ApplicationServices { get; }

        /// <summary>
        /// Registers a delegate as a middleware component.
        /// </summary>
        /// <param name="middleware">
        /// A delegate that implements the middleware using a delegate.
        /// </param>
        /// <returns>A reference to this instance</returns>
        public PipelineBuilder Use(Func<RequestContext, PipelineDelegate, CancellationToken, Task> middleware)
        {
            ApplicationServices.AddScoped<IMiddleware>(_ => new MiddlewareWrapper(middleware));
            return this;
        }

        /// <summary>
        /// Registers a middleware component.
        /// </summary>
        /// <typeparam name="T">Middleware component type</typeparam>
        /// <returns>A reference to this instance</returns>
        public PipelineBuilder UseMiddleware<T>() where T : class, IMiddleware
        {
            ApplicationServices.AddScoped<IMiddleware, T>();
            return this;
        }

        /// <summary>
        /// Registers creation of a middleware component.
        /// </summary>
        /// <param name="implementationFactory">A function that creates the middleware instance.</param>
        /// <typeparam name="T">Middleware component type</typeparam>
        /// <returns>A reference to this instance</returns>
        public PipelineBuilder UseMiddleware<T>(Func<IServiceProvider, T> implementationFactory) where T : IMiddleware
        {
            ApplicationServices.AddScoped<IMiddleware>(serviceProvider => implementationFactory(serviceProvider));
            return this;
        }
    }
}