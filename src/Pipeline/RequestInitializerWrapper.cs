using System;

namespace Vertical.ConsoleApplications.Pipeline
{
    /// <summary>
    /// Wraps an action that performs request initialization.
    /// </summary>
    internal sealed class RequestInitializerWrapper : IRequestInitializer
    {
        private readonly Action<RequestContext> _initializeAction;

        internal RequestInitializerWrapper(Action<RequestContext> initializeAction)
        {
            _initializeAction = initializeAction ?? throw new ArgumentNullException(nameof(initializeAction));
        }

        /// <inheritdoc />
        public void Initialize(RequestContext context) => _initializeAction(context);
    }
}