using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Pipeline;

namespace Vertical.ConsoleApplications.Routing
{
    internal class CommandRouter : ICommandRouter
    {
        private readonly List<RouteDescriptor> _routeDescriptors;
        private readonly ILogger<CommandRouter>? _logger;
        private readonly IComparer<RouteDescriptor> _descriptorComparer = new RouteDescriptorComparer();

        private sealed class RouteDescriptorComparer : IComparer<RouteDescriptor>
        {
            /// <inheritdoc />
            public int Compare(RouteDescriptor? x, RouteDescriptor? y)
            {
                return string.Compare(x?.Route, y?.Route, StringComparison.CurrentCulture);
            }
        }

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="routeDescriptors">Route descriptors</param>
        /// <param name="logger">Logger</param>
        public CommandRouter(
            IEnumerable<RouteDescriptor> routeDescriptors,
            ILogger<CommandRouter>? logger = null)
        {
            _routeDescriptors = routeDescriptors
                .OrderBy(dsc => dsc.Route)
                .ToList();
            
            _logger = logger;
        }
        
        /// <inheritdoc />
        public Task RouteAsync(IServiceProvider serviceProvider,
            CommandContext context, 
            CancellationToken cancellationToken)
        {
            var route = context.OriginalFormat;
            var handlerMatched = TryGetFactory(context.OriginalFormat, out var factory);

            if (!handlerMatched)
            {
                _logger.LogInformation("No matching handler for command route '{route}'", route);
                return Task.CompletedTask;
            }

            var handler = factory!(serviceProvider);
            
            _logger.LogInformation("Matched handler for command route '{route}' = {handler}",
                route,
                handler);

            var subContext = new CommandContext(
                context.Arguments.Skip(1).ToArray(),
                serviceProvider,
                context.Data);

            return handler.HandleAsync(subContext, cancellationToken);
        }

        private bool TryGetFactory(string route, out Func<IServiceProvider, ICommandHandler>? factory)
        {
            factory = null;

            if (string.IsNullOrWhiteSpace(route))
                return false;
            
            var matchDescriptor = new RouteDescriptor(route, default!);
            var index = _routeDescriptors.BinarySearch(matchDescriptor, _descriptorComparer);

            if (index > -1)
            {
                factory = _routeDescriptors[index].ImplementationFactory;
                return true;
            }

            index = Math.Min(~index, _routeDescriptors.Count - 1);

            for (; index >= 0; index--)
            {
                var (map, implementationFactory) = _routeDescriptors[index];

                if (route.StartsWith(map))
                {
                    factory = implementationFactory;
                    return true;
                }

                if (route[0] > map[0])
                    return false;
            } 

            return false;
        }
    }
}