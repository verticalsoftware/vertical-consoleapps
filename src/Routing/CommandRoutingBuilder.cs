using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Middleware;
using Vertical.ConsoleApplications.Pipeline;

namespace Vertical.ConsoleApplications.Routing
{
    using HandlerDelegate = Func<string[], CancellationToken, Task>;

    /// <summary>
    /// Used to configure how commands are routed.
    /// </summary>
    public class CommandRoutingBuilder
    {
        private readonly ApplicationPipelineBuilder _applicationBuilder;

        internal CommandRoutingBuilder(ApplicationPipelineBuilder applicationBuilder)
        {
            _applicationBuilder = applicationBuilder;
        }

        /// <summary>
        /// Registers middleware that uses maps between commands and delegate
        /// functions to handle implementation logic.
        /// </summary>
        /// <param name="maps"></param>
        /// <returns>A reference to this instance.</returns>
        public CommandRoutingBuilder MapCommands(params (string command, HandlerDelegate handler)[] maps)
        {
            if (maps.Length == 0)
                return this;

            _applicationBuilder.UseMiddleware<CommandDelegateMiddleware>(
                maps.ToDictionary(t => t.command, t => t.handler),
                _applicationBuilder.ServiceProvider.GetService<ILogger<CommandDelegateMiddleware>>());

            return this;
        }

        /// <summary>
        /// Registers middleware that uses implementations of 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public CommandRoutingBuilder RouteToHandlers()
        {
            var router = _applicationBuilder.ServiceProvider.GetService<ICommandRouter>()
                ??
                throw new InvalidOperationException("Cannot map command handlers because the required routing "
                                                    + "services are not registered. ");

            _applicationBuilder.UseMiddleware<CommandRoutingMiddleware>(router);
            
            return this;
        }
    }
}