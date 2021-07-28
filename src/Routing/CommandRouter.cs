using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vertical.ConsoleApplications.Extensions;

namespace Vertical.ConsoleApplications.Routing
{
    internal class CommandRouter : ICommandRouter
    {
        private readonly ILogger<CommandRouter>? logger;
        private readonly IOptions<RoutingConfiguration> options;
        private readonly IServiceProvider services;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="options">Options</param>
        /// <param name="services">Services</param>
        public CommandRouter(
            IOptions<RoutingConfiguration> options,
            IServiceProvider services,
            ILogger<CommandRouter>? logger = null)
        {
            this.logger = logger;
            this.options = options;
            this.services = services;
        }
        
        /// <inheritdoc />
        public async Task<bool> TryRouteCommandAsync(string[] arguments, CancellationToken cancellationToken)
        {
            var configuration = options.Value;
            var route = string.Empty;
            
            logger?.LogTrace("Try routing command from {count} arguments", arguments.Length);

            for (var c = 0; c < arguments.Length; c++)
            {
                route = Arguments.Combine(route, arguments[c]);
                
                logger?.LogTrace("Evaluating route candidate '{route}'", route);

                if (!configuration.RouteMap.TryGetValue(route, out var metadata))
                {
                    logger?.LogTrace("Route not matched");
                    continue;
                }

                logger?.LogTrace("Matched route '{route}' to metadata entry"
                                + Environment.NewLine
                                + "  Declaring type : {hostType}"
                                + Environment.NewLine
                                + "  Handler method : {method}",
                    route,
                    metadata.HostType,
                    metadata.MethodMetadata.Name);

                using var dependencyScope = services.CreateScope();
                
                var routeHost = dependencyScope.ServiceProvider.GetRequiredService(metadata.HostType);
                var argumentSubset = arguments.Skip(c + 1).ToArray();

                logger?.LogTrace("Invoking metadata target method with argument subset of {count}"
                                 + Environment.NewLine
                                 + string.Join(Environment.NewLine, argumentSubset.Select((arg, i) => $"  {i}> \"{arg}\"")),
                    argumentSubset.Length);
                
                await metadata.InvokeTarget(routeHost, argumentSubset, cancellationToken);

                return true;
            }

            logger?.LogTrace("[CommandRouter] No routes matched for arguments: [{arguments}]",
                string.Join(",", arguments));

            return false;
        }
    }
}