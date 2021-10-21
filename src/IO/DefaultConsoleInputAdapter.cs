using System;

namespace Vertical.ConsoleApplications.IO
{
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