using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Providers;

namespace Vertical.ConsoleApplications
{
    public class ConsoleHostedService : IHostedService
    {
        private readonly ILogger<ConsoleHostedService> logger;
        private readonly IHostApplicationLifetime hostApplicationLifetime;
        private readonly IEnumerable<ICommandProvider> commandProviders;
        private readonly IPipelineOrchestrator pipelineOrchestrator;

        public ConsoleHostedService(ILogger<ConsoleHostedService> logger,
            IHostApplicationLifetime hostApplicationLifetime,
            IEnumerable<ICommandProvider> commandProviders,
            IPipelineOrchestrator pipelineOrchestrator)
        {
            this.logger = logger;
            this.hostApplicationLifetime = hostApplicationLifetime;
            this.commandProviders = commandProviders;
            this.pipelineOrchestrator = pipelineOrchestrator;
        }
        
        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting console host");
            
            hostApplicationLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(() => InvokeCommandProvidersAsync(cancellationToken), cancellationToken);
            });

            return Task.CompletedTask;
        }

        private async Task InvokeCommandProvidersAsync(CancellationToken cancellationToken)
        {
            foreach (var commandProvider in commandProviders)
            {
                await commandProvider.ExecuteCommandsAsync(arguments => pipelineOrchestrator.ExecutePipelineAsync(
                        arguments, 
                        cancellationToken),
                    cancellationToken);
            }
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping console host");

            return Task.CompletedTask;
        }
    }
}