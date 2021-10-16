using System;

namespace Vertical.ConsoleApplications.IO
{
    internal class DefaultConsoleAdapter : IConsoleAdapter
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

        /// <inheritdoc />
        public void Write(string str)
        {
            Console.Write(str);
        }

        /// <inheritdoc />
        public void WriteLine(string str)
        {
            Console.WriteLine(str);
        }
    }
}