using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Vertical.ConsoleApplications.Utilities
{
    internal static class ArgumentHelpers
    {
        private const string TokenPattern = @"(?<!\\)\$(\w+)";
        
        internal static string[] SplitFromString(string str)
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

        internal static bool IsCommandMatch(string[] args, string command)
        {
            var commandLength = command.Length;
            var offset = 0;

            for (var c = 0; c < args.Length && offset < command.Length; c++)
            {
                var arg = args[c];
                var argLength = arg.Length;
                var compareLength = commandLength - offset;

                if (compareLength < argLength || string.Compare(arg, 0, command, offset, argLength) != 0)
                    return false;

                offset += argLength + 1;
            }

            return true;
        }

        internal static string ReplaceEnvironmentVariables(string arg)
        {
            return Regex.Replace(arg, TokenPattern, match => Environment.GetEnvironmentVariable(
                match.Groups[1].Value) ?? arg);
        }

        internal static string ReplaceSpecialFolderPaths(string arg)
        {
            return Regex.Replace(arg, TokenPattern, match =>
                Enum.TryParse(match.Groups[1].Value, ignoreCase: true, out Environment.SpecialFolder specialFolder)
                    ? Environment.GetFolderPath(specialFolder)
                    : arg);
        }
    }
}