using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Vertical.ConsoleApplications.Pipeline
{
    /// <summary>
    /// Represents the context available to the argument pipeline.
    /// </summary>
    public class CommandContext
    {
        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="arguments">Context arguments</param>
        /// <param name="services">Request service provider</param>
        /// <param name="data">Gets additional data associated with the context</param>
        public CommandContext(string[] arguments,
            IServiceProvider services,
            object? data)
        {
            Arguments = arguments;
            Services = services;
            Data = data;
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
        public IHostApplicationLifetime ApplicationLifetime => Services.GetRequiredService<IHostApplicationLifetime>();

        /// <summary>
        /// Gets the original format.
        /// </summary>
        public string OriginalFormat => string.Join(' ', Arguments);

        /// <summary>
        /// Gets application defined data associated with the context.
        /// </summary>
        public object? Data { get; }

        /// <inheritdoc />
        public override string ToString() => OriginalFormat;
    }
}