namespace Vertical.ConsoleApplications.Pipeline
{
    internal sealed class DefaultContextDataFactory : IContextDataFactory
    {
        /// <inheritdoc />
        public object? CreateContextData(string[] arguments) => default;
    }
}