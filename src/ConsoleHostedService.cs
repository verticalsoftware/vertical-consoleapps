using System;
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
        private readonly CancellationTokenSource shutdownCancellationTokenSource = new();
        private Task runProvidersTask = default!;

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
            logger.LogTrace("Starting console host");

            var shutdownToken = shutdownCancellationTokenSource.Token;

            hostApplicationLifetime.ApplicationStarted.Register(() =>
            {
                runProvidersTask = Task.Run(() => InvokeCommandProvidersAsync(shutdownToken), shutdownToken);
            });

            return Task.CompletedTask;
        }

        private async Task InvokeCommandProvidersAsync(CancellationToken cancellationToken)
        {
            foreach (var commandProvider in commandProviders)
            {
                await commandProvider.ExecuteCommandsAsync(arguments => HandleProviderInvocation(
                    arguments, 
                    cancellationToken), 
                cancellationToken);
            }
        }

        private async Task HandleProviderInvocation(string[] arguments, CancellationToken cancellationToken)
        {
            try
            {
                await pipelineOrchestrator.ExecutePipelineAsync(arguments, cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unhandled error occurred in the command pipeline");
            }
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine();
            
            logger.LogTrace("Stopping console host");
            
            shutdownCancellationTokenSource.Cancel();

            await runProvidersTask;
        }
    }
}