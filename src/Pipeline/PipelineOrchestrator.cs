using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Pipeline
{
    public class PipelineOrchestrator : IPipelineOrchestrator
    {
        private readonly ILogger<PipelineOrchestrator> logger;
        private readonly IEnumerable<IPipelineTask<CommandContext>> pipeline;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="pipeline">Components that represent the pipeline tasks</param>
        public PipelineOrchestrator(ILogger<PipelineOrchestrator> logger,
            IEnumerable<IPipelineTask<CommandContext>> pipeline)
        {
            this.logger = logger;
            this.pipeline = pipeline;
        }
        
        /// <inheritdoc />
        public async Task ExecutePipelineAsync(string[] args, CancellationToken cancellationToken)
        {
            await PipelineDelegate.InvokeAllAsync(pipeline,
                new CommandContext(args),
                cancellationToken);
        }
    }
}