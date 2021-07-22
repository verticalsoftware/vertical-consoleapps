using System;
using System.Collections.Generic;
using System.Text;

namespace Vertical.ConsoleApplications.Extensions
{
    internal static class Strings
    {
        internal static IEnumerable<string> ToArgumentsArray(this string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
                yield break;

            const char doubleQuote = '"';
            var builder = new StringBuilder();
            var charQueue = new Queue<char>(str);
            var lastRead = char.MinValue;
            var openQuote = false;

            while (charQueue.TryDequeue(out var c))
            {
                switch (c)
                {
                    case doubleQuote when openQuote && lastRead == doubleQuote:
                        openQuote = false;
                        builder.Append(doubleQuote);
                        break;
                    
                    case doubleQuote when openQuote && lastRead != doubleQuote:
                        openQuote = false;
                        break;
                    
                    case doubleQuote when !openQuote && lastRead == doubleQuote:
                        throw new InvalidOperationException();
                    
                    case doubleQuote when !openQuote && lastRead != doubleQuote:
                        openQuote = true;
                        break;
                    
                    case ' ' when !openQuote:
                        if (builder.Length > 0)
                        {
                            yield return builder.ToString();
                        }
                        builder.Clear();
                        break;
                    
                    case { } when char.IsControl(c):
                        break;
                    
                    default:
                        builder.Append(c);
                        break;
                }

                lastRead = c;
            }

            if (builder.Length > 0)
            {
                yield return builder.ToString();
            }
        }
    }
}