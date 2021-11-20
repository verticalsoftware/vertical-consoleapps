using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vertical.ConsoleApplications.Routing;

namespace Vertical.ConsoleApplications.Pipeline
{
    /// <summary>
    /// Represents the context available to the argument pipeline.
    /// </summary>
    public class RequestContext
    {
        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="arguments">Context arguments</param>
        /// <param name="requestItems">Request items</param>
        /// <param name="hostApplicationLifetime">Host application lifetime</param>
        /// <param name="services">Request service provider</param>
        public RequestContext(string[] arguments,
            RequestItems requestItems,
            IHostApplicationLifetime hostApplicationLifetime, 
            IServiceProvider services)
        {
            Arguments = arguments;
            Services = services;
            ApplicationLifetime = hostApplicationLifetime;
            Items = requestItems;
        }
        
        /// <summary>
        /// Gets the context arguments.
        /// </summary>
        public string[] Arguments { get; }

        /// <summary>
        /// Gets the request services.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Gets the <see cref="IHostApplicationLifetime"/>.
        /// </summary>
        public IHostApplicationLifetime ApplicationLifetime { get; }

        /// <summary>
        /// Gets the current command request features.
        /// </summary>
        public RequestItems Items { get; }

        /// <summary>
        /// Gets the original format.
        /// </summary>
        public string OriginalFormat => string.Join(' ', Arguments);

        /// <inheritdoc />
        public override string ToString() => OriginalFormat;
    }
}