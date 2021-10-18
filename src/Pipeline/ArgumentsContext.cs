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
            CancellationTokenSource cancellationTokenSource)
        {
            CancellationTokenSource = cancellationTokenSource;
            ServiceProvider = serviceProvider;
            Arguments = new List<string>(arguments);
        }
        
        private IServiceProvider ServiceProvider { get; }
        
        private CancellationTokenSource CancellationTokenSource { get; }

        /// <summary>
        /// Gets the application arguments.
        /// </summary>
        public List<string> Arguments { get; }

        /// <summary>
        /// Gets a token that can be observed for cancellation requests.
        /// </summary>
        public CancellationToken CancellationToken => CancellationTokenSource.Token;

        /// <summary>
        /// Signals for the application to exit.
        /// </summary>
        public void StopApplication()
        {
            CancellationTokenSource.Cancel();
        }

        /// <inheritdoc />
        public override string ToString() => string.Join(' ', Arguments);

        IServiceProvider IApplicationServices.ApplicationServices => ServiceProvider;
    }
}