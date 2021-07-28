using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Vertical.ConsoleApplications.Routing;

namespace Vertical.ConsoleApplications
{
    public static class RoutingExtensions
    {
        /// <summary>
        /// Adds services required for command routing.
        /// </summary>
        /// <param name="services">Services collection</param>
        /// <param name="assembly">Assembly to scan for command handlers. If omitted, the calling
        /// assembly is used.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddCommandRouting(this IServiceCollection services,
            Assembly? assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            var routeHostTypes = new HashSet<Type>(RouteMetadataBuilder.ScanForTypes(assembly));

            // Register route hosts
            foreach (var type in routeHostTypes)
            {
                services.TryAddScoped(type);
            }

            // Add command routing services
            services.AddSingleton<ICommandRouter, CommandRouter>();

            // Add routing maps to configuration
            services.Configure<RoutingConfiguration>(config =>
            {
                var entries = routeHostTypes.SelectMany(RouteMetadataBuilder.BuildForType);

                var routeMap = config.RouteMap;

                foreach (var entry in entries)
                {
                    if (routeMap.TryGetValue(entry.AggregatedRoute, out var previousEntry))
                    {
                        if (previousEntry.Equals(entry))
                        {
                            // Keep this call idempotent
                            continue;
                        }

                        var message =
                            "Command route mapped to multiple handlers:"
                            + Environment.NewLine
                            + $"\t{entry}"
                            + Environment.NewLine
                            + $"\t{previousEntry}";

                        throw new InvalidOperationException(message);
                    }
                    
                    routeMap.Add(entry.AggregatedRoute, entry);
                }
            });
            
            return services;
        }
    }
}