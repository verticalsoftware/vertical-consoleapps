using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Providers;
using Vertical.ConsoleApplications.Services;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications
{
    /// <summary>
    /// Defines the hosted service implementation for the console application.
    /// </summary>
    public class ConsoleHostedService : IHostedService
    {
        private readonly ILogger<ConsoleHostedService>? _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IEnumerable<IArgumentsProvider> _argumentsProviders;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEnumerable<IRequestInitializer> _requestInitializers;
        private readonly IStartupTaskRunner? _startupTaskRunner;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        public ConsoleHostedService(
            IHostApplicationLifetime hostApplicationLifetime,
            IServiceProvider serviceProvider,
            IEnumerable<IArgumentsProvider> argumentsProviders,
            IEnumerable<IRequestInitializer> requestInitializers,
            IStartupTaskRunner? startupTaskRunner = null,
            ILogger<ConsoleHostedService>? logger = null)
        {
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
            _argumentsProviders = argumentsProviders;
            _serviceProvider = serviceProvider;
            _requestInitializers = requestInitializers;
            _startupTaskRunner = startupTaskRunner;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger?.LogInformation("Starting console host");

            _hostApplicationLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () =>
                {
                    using var cancelTokenSource = new CancellationTokenSource();

                    try
                    {
                        if (_startupTaskRunner != null)
                        {
                            await _startupTaskRunner.RunStartupTasksAsync();
                        }
                        
                        await InvokeCommandProvidersAsync(cancelTokenSource.Token);
                    }
                    catch (Exception exception)
                    {
                        _logger?.LogError(exception, "Unhandled exception occurred");
                    }
                    finally
                    {
                        _hostApplicationLifetime.StopApplication();
                    }
                }, cancellationToken);
            });

            return Task.CompletedTask;
        }

        private async Task InvokeCommandProvidersAsync(CancellationToken cancellationToken)
        {
            foreach (var provider in _argumentsProviders.TakeWhile(_ => !cancellationToken.IsCancellationRequested))
            {
                await provider.InvokeArgumentsAsync(async args =>
                {
                    using var scope = _serviceProvider.CreateScope();

                    var pipelineFactory = scope.ServiceProvider.GetRequiredService<IPipelineFactory<RequestContext>>();

                    var pipeline = pipelineFactory.CreatePipeline();

                    var context = new RequestContext(args, new RequestItems(), _hostApplicationLifetime , scope.ServiceProvider);

                    foreach (var initializer in _requestInitializers)
                    {
                        initializer.Initialize(context);
                    }

                    await pipeline(context, cancellationToken);

                }, cancellationToken);
            }
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}