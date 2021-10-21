using System;
using System.Diagnostics.CodeAnalysis;

namespace Vertical.ConsoleApplications.IO
{
    [ExcludeFromCodeCoverage]
    internal class DefaultConsoleInputAdapter : IConsoleInputAdapter
    {
        /// <inheritdoc />
        public string? ReadLine()
        {
            return Console.ReadLine();
        }

        /// <inheritdoc />
        public ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }
    }
}