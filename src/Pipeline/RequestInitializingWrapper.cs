using System;

namespace Vertical.ConsoleApplications.Pipeline
{
    /// <summary>
    /// Wraps an action that performs request initialization.
    /// </summary>
    internal sealed class RequestInitializingWrapper : IRequestInitializer
    {
        private readonly Action<RequestContext> _initializeAction;

        internal RequestInitializingWrapper(Action<RequestContext> initializeAction)
        {
            _initializeAction = initializeAction ?? throw new ArgumentNullException(nameof(initializeAction));
        }

        /// <inheritdoc />
        public void Initialize(RequestContext context) => _initializeAction(context);
    }
}