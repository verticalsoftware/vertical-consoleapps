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
        internal ArgumentsContext(string[] arguments,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            ServiceProvider = serviceProvider;
            Arguments = new List<string>(arguments);
            CancellationToken = cancellationToken;
        }
        
        private IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the application arguments.
        /// </summary>
        public List<string> Arguments { get; }

        /// <summary>
        /// Gets a token that can be observed for cancellation requests.
        /// </summary>
        public CancellationToken CancellationToken { get; }
        
        /// <summary>
        /// Gets or sets whether to request application stoppage.
        /// </summary>
        public bool RequestApplicationStop { get; set; }

        /// <inheritdoc />
        public override string ToString() => string.Join(' ', Arguments);

        IServiceProvider IApplicationServices.ApplicationServices => ServiceProvider;
    }
}