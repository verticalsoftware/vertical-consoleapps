using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Pipeline
{
    public class PipelineOrchestrator : IPipelineOrchestrator
    {
        private readonly ILogger<PipelineOrchestrator>? logger;
        private readonly IEnumerable<IPipelineTask<CommandContext>> pipeline;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="pipeline">Components that represent the pipeline tasks</param>
        public PipelineOrchestrator(IEnumerable<IPipelineTask<CommandContext>> pipeline,
            ILogger<PipelineOrchestrator>? logger = null)
        {
            this.logger = logger;
            this.pipeline = pipeline;
        }

        /// <inheritdoc />
        public async Task ExecutePipelineAsync(string[] args, CancellationToken cancellationToken)
        {
            if (true == logger?.IsEnabled(LogLevel.Trace))
            {
                logger?.LogTrace(
                    "Sending {count} arguments through pipeline:" 
                    + Environment.NewLine 
                    + string.Join(Environment.NewLine, pipeline.Select((t, i) => $"  {i}> {t}")),
                    pipeline.Count());
            }
                
            await PipelineDelegate.InvokeAllAsync(pipeline,
                new CommandContext(args),
                cancellationToken);
        }
    }
}