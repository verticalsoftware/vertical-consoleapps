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
        private readonly ILogger<ConsoleHostedService>? _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly PipelineDelegate<ArgumentsContext> _pipelineDelegate;
        private readonly IEnumerable<IArgumentsProvider> _argumentsProviders;
        private readonly IServiceProvider _serviceProvider;

        public ConsoleHostedService(
            IHostApplicationLifetime hostApplicationLifetime,
            PipelineDelegate<ArgumentsContext> pipelineDelegate,
            IEnumerable<IArgumentsProvider> argumentsProviders,
            IServiceProvider serviceProvider,
            ILogger<ConsoleHostedService>? logger = null)
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

            _hostApplicationLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () =>
                {
                    using var cancelTokenSource = new CancellationTokenSource();

                    try
                    {
                        await InvokeCommandProvidersAsync(cancelTokenSource);
                    }
                    catch (Exception exception)
                    {
                        _logger?.LogError(exception, "Unhandled exception occurred");
                    }
                    finally
                    {
                        _hostApplicationLifetime.StopApplication();
                    }
                });
            });

            return Task.CompletedTask;
        }

        private async Task InvokeCommandProvidersAsync(CancellationTokenSource cts)
        {
            foreach (var provider in _argumentsProviders)
            {
                await provider.InvokeArgumentsAsync(async (args, ct) =>
                {
                    using var scope = _serviceProvider.CreateScope();

                    var context = new ArgumentsContext(args, _serviceProvider, ct);
                    
                    await _pipelineDelegate(context);

                    if (context.RequestApplicationStop)
                    {
                        cts.Cancel();
                    }
                    
                }, cts.Token);
            }
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}