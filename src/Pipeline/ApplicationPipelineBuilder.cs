using System;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Pipeline
{
    /// <summary>
    /// Used to construct the console application pipeline.
    /// </summary>
    public class ApplicationPipelineBuilder : PipelineBuilder<ArgumentsContext>
    {
        internal ApplicationPipelineBuilder(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
        
        /// <summary>
        /// Gets the root application service provider used for internal
        /// registration extensions.
        /// </summary>
        internal IServiceProvider ServiceProvider { get; }
    }
}