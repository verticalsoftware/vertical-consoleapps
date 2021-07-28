using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Routing
{
    /// <summary>
    /// Defines metadata for a specific command route.
    /// </summary>
    public class RouteMetadata
    {
        internal RouteMetadata(
            CommandRouteAttribute? hostRoute,
            CommandRouteAttribute handlerRoute,
            Type hostType,
            MethodInfo methodMetadata,
            Func<object, string[], CancellationToken, Task> invokeTarget,
            string aggregatedRoute)
        {
            HostRoute = hostRoute;
            HandlerRoute = handlerRoute;
            HostType = hostType;
            MethodMetadata = methodMetadata;
            InvokeTarget = invokeTarget;
            AggregatedRoute = aggregatedRoute;
        }

        /// <summary>
        /// Gets the host route attribute, if defined.
        /// </summary>
        public CommandRouteAttribute? HostRoute { get; }
        
        /// <summary>
        /// Gets the handler route attribute.
        /// </summary>
        public CommandRouteAttribute HandlerRoute { get; }

        /// <summary>
        /// Gets the route host metadata.
        /// </summary>
        public Type HostType { get; }
        
        /// <summary>
        /// Gets the route method metadata.
        /// </summary>
        public MethodInfo MethodMetadata { get; }
        
        /// <summary>
        /// Gets the asynchronous function that implements the route logic.
        /// </summary>
        public Func<object, string[], CancellationToken, Task> InvokeTarget { get; }

        /// <summary>
        /// Gets the aggregated route.
        /// </summary>
        public string AggregatedRoute { get; }

        /// <inheritdoc />
        public override string ToString() => $"'{AggregatedRoute}' @{HostType}.{MethodMetadata.Name}";

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is RouteMetadata other
                                                    && AggregatedRoute == other.AggregatedRoute
                                                    && ReferenceEquals(MethodMetadata, other.MethodMetadata);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(AggregatedRoute, MethodMetadata);
    }
}