using Microsoft.Extensions.DependencyInjection;
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
        /// Adds the pipeline task necessary to route commands to controller handlers.
        /// </summary>
        /// <returns>A reference to this instance.</returns>
        public ApplicationBuilder UseCommandRouting()
        {
            services.AddSingleton<IPipelineTask<CommandContext>, RouteCommandTask>();
            return this;
        }
    }
}