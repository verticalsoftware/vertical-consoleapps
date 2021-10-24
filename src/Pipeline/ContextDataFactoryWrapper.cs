using System;

namespace Vertical.ConsoleApplications.Pipeline
{
    internal class ContextDataFactoryWrapper : IContextDataFactory
    {
        private readonly Func<string[], object?> _factory;

        internal ContextDataFactoryWrapper(Func<string[], object?> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <inheritdoc />
        public object? CreateContextData(string[] arguments) => _factory(arguments);
    }
}