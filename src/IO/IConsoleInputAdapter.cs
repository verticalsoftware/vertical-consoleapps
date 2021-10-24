using System;

namespace Vertical.ConsoleApplications.IO
{
    /// <summary>
    /// Abstracts input from a stream.
    /// </summary>
    public interface IConsoleInputAdapter
    {
        /// <summary>
        /// Reads a line a content from the input stream.
        /// </summary>
        /// <returns><see cref="string"/></returns>
        string? ReadLine();

        /// <summary>
        /// Reads a key from the input stream.
        /// </summary>
        /// <returns><see cref="ConsoleKeyInfo"/></returns>
        ConsoleKeyInfo ReadKey();
    }
}