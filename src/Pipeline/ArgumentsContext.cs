using System;
using System.Collections.Generic;
using System.Threading;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Pipeline
{
    /// <summary>
    /// Defines the contextual data that is available in the argument
    /// processing pipeline.
    /// </summary>
    public class ArgumentsContext : IApplicationServices
    {
        internal ArgumentsContext(IReadOnlyList<string> arguments,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            ServiceProvider = serviceProvider;
            Arguments = arguments;
            CancellationToken = cancellationToken;
        }
        
        private IServiceProvider ServiceProvider { get; }
        
        /// <summary>
        /// Gets the application arguments.
        /// </summary>
        public IReadOnlyList<string> Arguments { get; }
        
        /// <summary>
        /// Gets a token that can be observed for cancellation requests.
        /// </summary>
        public CancellationToken CancellationToken { get; }

        /// <inheritdoc />
        public override string ToString() => string.Join(' ', Arguments);

        IServiceProvider IApplicationServices.ApplicationServices => ServiceProvider;
    }
}