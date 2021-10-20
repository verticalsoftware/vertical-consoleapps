using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Pipeline;

namespace Vertical.ConsoleApplications.Routing
{
    using HandlerDelegate = System.Func<string[], System.Threading.CancellationToken, System.Threading.Tasks.Task>;

    public class CommandRoutingBuilder : IDisposable
    {
        private readonly ApplicationPipelineBuilder _applicationBuilder;
        private readonly Dictionary<string, HandlerDelegate> _handlerMap = new();

        internal CommandRoutingBuilder(ApplicationPipelineBuilder applicationBuilder)
        {
            _applicationBuilder = applicationBuilder;
        }

        /// <summary>
        /// Registers a delegate that handles a specific command implementation.
        /// </summary>
        /// <param name="command">Command to match</param>
        /// <param name="handler">Handler implementation</param>
        /// <returns>A reference to this instance</returns>
        public CommandRoutingBuilder MapCommand(string command, HandlerDelegate handler)
        {
            _handlerMap[command] = handler;
            return this;
        }

        void IDisposable.Dispose()
        {
            _applicationBuilder.UseMiddleware<CommandHandlerMiddleware>(
                _handlerMap,
                _applicationBuilder.ServiceProvider.GetService<ILogger<CommandHandlerMiddleware>>());
        }
    }
}