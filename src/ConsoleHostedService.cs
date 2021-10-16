using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Providers;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications
{
    public class ConsoleHostedService : IHostedService
    {
        private readonly ILogger<ConsoleHostedService> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly PipelineDelegate<ArgumentsContext> _pipelineDelegate;
        private readonly IEnumerable<IArgumentsProvider> _argumentsProviders;
        private readonly IServiceProvider _serviceProvider;
        private readonly CancellationTokenSource _shutdownCancellationTokenSource = new();
        private Task _runProvidersTask = default!;

        public ConsoleHostedService(ILogger<ConsoleHostedService> logger,
            IHostApplicationLifetime hostApplicationLifetime,
            PipelineDelegate<ArgumentsContext> pipelineDelegate,
            IEnumerable<IArgumentsProvider> argumentsProviders,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
            _pipelineDelegate = pipelineDelegate;
            _argumentsProviders = argumentsProviders;
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("Starting console host");

            var shutdownToken = _shutdownCancellationTokenSource.Token;

            _hostApplicationLifetime.ApplicationStarted.Register(() =>
            {
                _runProvidersTask = Task.Run(() => InvokeCommandProvidersAsync(shutdownToken), shutdownToken);
            });

            return Task.CompletedTask;
        }

        private async Task InvokeCommandProvidersAsync(CancellationToken cancellationToken)
        {
            foreach (var provider in _argumentsProviders)
            {
                await provider.InvokeArgumentsAsync(async (args, ct) =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    await _pipelineDelegate(new ArgumentsContext(args, _serviceProvider, ct));
                }, cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine();
            
            _logger.LogTrace("Stopping console host");
            
            _shutdownCancellationTokenSource.Cancel();

            await _runProvidersTask;
        }
    }
}