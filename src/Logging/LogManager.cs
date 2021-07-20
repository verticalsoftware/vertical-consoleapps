using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Vertical.ConsoleApplications.Logging
{
    internal static class LogManager
    {
        /// <summary>
        /// Gets or sets the logger factory.
        /// </summary>
        internal static ILoggerFactory LoggerFactory { get; set; } = NullLoggerFactory.Instance;

        /// <summary>
        /// Creates a logger
        /// </summary>
        /// <typeparam name="T">The logger category type</typeparam>
        /// <returns><see cref="ILogger"/></returns>
        internal static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();

        /// <summary>
        /// Creates a logger
        /// </summary>
        /// <param name="categoryName">The logger category name</param>
        /// <returns><see cref="ILogger"/></returns>
        internal static ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);
    }
}