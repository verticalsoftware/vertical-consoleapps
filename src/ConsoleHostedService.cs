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

namespace Vertical.ConsoleApplications
{
    public class ConsoleHostedService : IHostedService
    {
        private readonly ILogger<ConsoleHostedService>? _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IEnumerable<IArgumentsProvider> _argumentsProviders;
        private readonly IServiceProvider _serviceProvider;
        private readonly IContextDataFactory _contextDataFactory;

        public ConsoleHostedService(
            IHostApplicationLifetime hostApplicationLifetime,
            IEnumerable<IArgumentsProvider> argumentsProviders,
            IServiceProvider serviceProvider,
            IContextDataFactory contextDataFactory,
            ILogger<ConsoleHostedService>? logger = null)
        {
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
            _argumentsProviders = argumentsProviders;
            _serviceProvider = serviceProvider;
            _contextDataFactory = contextDataFactory;
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
                }, cancellationToken);
            });

            return Task.CompletedTask;
        }

        private async Task InvokeCommandProvidersAsync(CancellationTokenSource cts)
        {
            var cancellationToken = cts.Token;
            
            foreach (var provider in _argumentsProviders.TakeWhile(_ => !cancellationToken.IsCancellationRequested))
            {
                await provider.InvokeArgumentsAsync(async args =>
                {
                    using var scope = _serviceProvider.CreateScope();

                    var middlewareFactory = scope.ServiceProvider.GetRequiredService<IMiddlewareFactory>();

                    var pipeline = middlewareFactory.Create();

                    var context = new CommandContext(args,
                        scope.ServiceProvider,
                        _contextDataFactory.CreateContextData(args));

                    await pipeline(context, cancellationToken);

                }, cts.Token);
            }
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}