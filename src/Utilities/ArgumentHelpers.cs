using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Vertical.ConsoleApplications.Utilities
{
    /// <summary>
    /// Defines methods used to work with arguments and argument strings.
    /// </summary>
    public static class ArgumentHelpers
    {
        /// <summary>
        /// Splits the given string into an argument array.
        /// </summary>
        /// <param name="str">The string to split.</param>
        /// <returns>The argument array.</returns>
        /// <remarks>
        /// This function will also not split within quoted strings.
        /// </remarks>
        public static string[] SplitFromString(string str)
        {
            var arguments = new List<string>(16);
            var span = str.AsSpan();
            var quoting = false;

            static int Push(List<string> list, ref ReadOnlySpan<char> src, int i)
            {
                if (i > 0)
                {
                    var str = src[..i].ToString().Replace("\"", string.Empty).Trim();

                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        list.Add(str);
                    }
                }

                src = i < src.Length ? src[i..] : string.Empty.AsSpan();
                return 0;
            }

            for (var c = 0; c < span.Length; c++)
            {
                switch (span[c])
                {
                    case ' ' when !quoting:
                        c = Push(arguments, ref span, c);
                        break;
                    
                    case '"':
                        quoting = !quoting;
                        break;
                }
            }

            Push(arguments, ref span, span.Length);

            return arguments.ToArray();
        }
    }
}