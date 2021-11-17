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
        private readonly Lazy<IHostApplicationLifetime> _lazyApplicationLifetime;
        private readonly Lazy<RequestItems> _lazyRequestFeatures;
        
        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="arguments">Context arguments</param>
        /// <param name="services">Request service provider</param>
        public RequestContext(string[] arguments, IServiceProvider services)
        {
            Arguments = arguments;
            Services = services;
            
            _lazyApplicationLifetime = new Lazy<IHostApplicationLifetime>(
                () => services.GetRequiredService<IHostApplicationLifetime>());
            
            _lazyRequestFeatures = new Lazy<RequestItems>(
                () => services.GetRequiredService<RequestItems>());
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
        public IHostApplicationLifetime ApplicationLifetime => _lazyApplicationLifetime.Value;

        /// <summary>
        /// Gets the current command request features.
        /// </summary>
        public RequestItems Items => _lazyRequestFeatures.Value;

        /// <summary>
        /// Gets the original format.
        /// </summary>
        public string OriginalFormat => string.Join(' ', Arguments);

        /// <inheritdoc />
        public override string ToString() => OriginalFormat;
    }
}