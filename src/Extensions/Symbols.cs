using System;
using System.Text.RegularExpressions;

namespace Vertical.ConsoleApplications.Extensions
{
    internal static class Symbols
    {
        /// <summary>
        /// Replaces symbols (e.g. $VARIABLE) in a string.
        /// </summary>
        /// <param name="str">String input</param>
        /// <param name="callback">A function that provides a value given the symbol</param>
        /// <returns>Replaced string</returns>
        internal static string? ReplaceSymbols(this string? str, Func<string, string?> callback)
        {
            if (str == null) return null;
            
            return Regex.Replace(str, @"\$\(?([a-zA-Z][a-zA-Z0-9_]+)\)?", eval =>
            {
                var symbol = eval.Groups[1].Value;
                var result = callback(symbol);

                return result ?? eval.Value;
            });
        }
    }
}