using System;
using System.Text.RegularExpressions;

namespace Vertical.ConsoleApplications.Utilities
{
    internal static class TokenHelpers
    {
        private const string TokenPattern = @"(?<!\\)\$\((\w+)\)";

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