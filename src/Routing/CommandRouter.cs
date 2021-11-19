using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Utilities;

namespace Vertical.ConsoleApplications.Routing
{
    internal class CommandRouter : ICommandRouter
    {
        private readonly List<RouteDescriptor> _routeDescriptors;
        private readonly ILogger<CommandRouter>? _logger;
        private readonly IComparer<RouteDescriptor> _descriptorComparer = new RouteDescriptorComparer();

        private sealed class RouteDescriptorComparer : IComparer<RouteDescriptor>
        {
            int IComparer<RouteDescriptor>.Compare(RouteDescriptor? x, RouteDescriptor? y)
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
            RequestContext context, 
            CancellationToken cancellationToken)
        {
            var route = context.OriginalFormat;
            var handlerMatched = TryGetDescriptor(context.OriginalFormat, out var descriptor);

            if (!handlerMatched)
            {
                _logger.LogInformation("No matching handler for command route '{route}'", route);
                return Task.CompletedTask;
            }

            var handler = descriptor!.ImplementationFactory(serviceProvider);
            var matchedRoute = descriptor.Route;
            
            context.Items.Set(descriptor);

            _logger.LogInformation("Matched handler for command route '{route}' = {handler}",
                route,
                handler);
            
            // Make new arguments to trim off command
            var trimmedFormat = context.OriginalFormat.Length > matchedRoute.Length
                ? context.OriginalFormat[matchedRoute.Length..]
                : context.OriginalFormat;

            var subContext = new RequestContext(
                ArgumentHelpers.SplitFromString(trimmedFormat),
                serviceProvider);

            return handler.HandleAsync(subContext, cancellationToken);
        }

        private bool TryGetDescriptor(string route, out RouteDescriptor? descriptor)
        {
            descriptor = null;
            
            if (string.IsNullOrWhiteSpace(route))
                return false;
            
            var matchDescriptor = new RouteDescriptor(route, default!);
            var index = _routeDescriptors.BinarySearch(matchDescriptor, _descriptorComparer);

            if (index > -1)
            {
                descriptor = _routeDescriptors[index];
                return true;
            }

            index = Math.Min(~index, _routeDescriptors.Count - 1);

            for (; index >= 0; index--)
            {
                descriptor = _routeDescriptors[index];
                var map = descriptor.Route;

                if (route.StartsWith(map))
                {
                    return true;
                }

                if (route[0] > map[0])
                    return false;
            } 

            return false;
        }
    }
}