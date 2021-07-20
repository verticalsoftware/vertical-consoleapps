using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Logging;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Pipeline
{
    public class PipelineOrchestrator : IPipelineOrchestrator
    {
        private readonly ILogger logger = LogManager.CreateLogger<PipelineOrchestrator>();
        private readonly IEnumerable<IPipelineTask<string[]>> pipeline;

        public PipelineOrchestrator(IEnumerable<IPipelineTask<string[]>> pipeline)
        {
            this.pipeline = pipeline;
        }
        
        /// <inheritdoc />
        public async Task ExecutePipelineAsync(string[] arguments, CancellationToken cancellationToken)
        {
            try
            {
                await PipelineDelegate.InvokeAllAsync(pipeline, arguments, cancellationToken);
            }
            finally
            {
                
            }
        }
    }
}