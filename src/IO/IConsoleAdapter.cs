using System;

namespace Vertical.ConsoleApplications.IO
{
    /// <summary>
    /// Abstracts input and output.
    /// </summary>
    public interface IConsoleAdapter
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

        /// <summary>
        /// Writes content to the output stream.
        /// </summary>
        /// <param name="str">Content to write.</param>
        void Write(string str);

        /// <summary>
        /// Writes content to the output stream followed by the current environment
        /// new line character.
        /// </summary>
        /// <param name="str">Content to write.</param>
        void WriteLine(string str);
    }
}