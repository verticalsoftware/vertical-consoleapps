namespace Vertical.ConsoleApplications.Pipeline
{
    /// <summary>
    /// Represents an object that creates context objects for each request.
    /// </summary>
    public interface IContextDataFactory
    {
        /// <summary>
        /// When implemented by a class, creates a context for the current command.
        /// </summary>
        /// <param name="arguments">Current request arguments</param>
        /// <returns><see cref="CommandContext"/> to be used in the middleware pipeline.</returns>
        object? CreateContextData(string[] arguments);
    }
}