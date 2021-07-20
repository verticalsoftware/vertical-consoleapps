using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Providers;

namespace Vertical.ConsoleApplications
{
    /// <summary>
    /// Represents a <see cref="IHostedService"/> implementation for the command line.
    /// </summary>
    public class ConsoleService : IHostedService
    {
        private readonly IHostApplicationLifetime applicationLifetime;
        private readonly IEnumerable<IArgumentProvider> argumentProviders;
        private readonly IPipelineOrchestrator pipelineOrchestrator;

        public ConsoleService(IHostApplicationLifetime applicationLifetime,
            IEnumerable<IArgumentProvider> argumentProviders,
            IPipelineOrchestrator pipelineOrchestrator)
        {
            this.applicationLifetime = applicationLifetime;
            this.argumentProviders = argumentProviders;
            this.pipelineOrchestrator = pipelineOrchestrator;
        }
        
        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            applicationLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () =>
                {
                    foreach (var provider in argumentProviders)
                    {
                        await provider.ExecuteInvocationsAsync(arguments => pipelineOrchestrator
                            .ExecutePipelineAsync(arguments, cancellationToken));
                    }
                }, cancellationToken);
            });

            return Task.CompletedTask;
        }
        
        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}