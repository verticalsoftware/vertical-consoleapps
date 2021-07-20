using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vertical.ConsoleApplications.Parsing
{
    /// <summary>
    /// Represents command argument parsing utilities. 
    /// </summary>
    public static class CommandArguments
    {
        /// <summary>
        /// Determines if any arguments match a template.
        /// </summary>
        /// <param name="arguments">Arguments</param>
        /// <param name="template">Template to match</param>
        /// <returns>True if any argument in the collection matches the template.</returns>
        public static bool GetSwitch(this IEnumerable<string> arguments, string template)
        {
            var templates = template.Split('|');
            return arguments.Any(arg => templates.Any(t => t == arg));
        }

        /// <summary>
        /// Gets arguments that are not preceded by a template
        /// </summary>
        /// <param name="arguments">Arguments</param>
        /// <param name="templatePrefixes">Template prefixes</param>
        /// <returns>A collection of arguments</returns>
        public static IEnumerable<string> GetArguments(this IEnumerable<string> arguments, 
            string templatePrefixes = "-|--|/")
        {
            var prefixesSplit = templatePrefixes.Split('|');
            var afterTemplate = false;
            var results = new List<string>();

            foreach (var argument in arguments)
            {
                switch (argument)
                {
                    case { } when prefixesSplit.Any(t => t == argument):
                        afterTemplate = true;
                        break;
                    
                    case { } when !afterTemplate:
                        results.Add(argument);
                        break;
                    
                    default:
                        afterTemplate = false;
                        break;
                }
            }

            return results;
        }

        /// <summary>
        /// Gets arguments that are not preceded by a template. The iteration stops when any template
        /// is matched.
        /// </summary>
        /// <param name="arguments">Arguments</param>
        /// <param name="templatePrefixes">Template prefixes</param>
        /// <returns>A collection of arguments.</returns>
        public static IEnumerable<string> GetLeadingArguments(this IEnumerable<string> arguments,
            string templatePrefixes = "-|--|/")
        {
            var prefixSplit = templatePrefixes.Split('|');

            return arguments.TakeWhile(argument => prefixSplit.Any(t => t != argument));
        }

        /// <summary>
        /// Attempts to get all values that follow any matched template.
        /// </summary>
        /// <param name="arguments">Arguments</param>
        /// <param name="template">The template that identifies the option</param>
        /// <returns>Option values</returns>
        public static IEnumerable<string> GetOptions(this IEnumerable<string> arguments, string template)
        {
            var queue = new Queue<string>(arguments);
            var templateSplit = template.Split('|');
            var results = new List<string>();
            var matched = false;

            while (queue.TryDequeue(out var argument))
            {
                switch (argument)
                {
                    case { } when matched:
                        results.Add(argument);
                        matched = false;
                        break;
                    
                    case { } when templateSplit.Any(t => t == argument):
                        matched = true;
                        break;
                }
            }

            return results;
        }

        /// <summary>
        /// Attempts to get an option value.
        /// </summary>
        /// <param name="arguments">Arguments</param>
        /// <param name="template">The template that identifies the option</param>
        /// <param name="value">The value assigned if the option was found</param>
        /// <returns>Whether the option was found</returns>
        public static bool TryGetOption(this IEnumerable<string> arguments,
            string template,
            out string? value)
        {
            var templateSplit = template.Split('|');
            
            value = arguments
                .SkipWhile(arg => templateSplit.All(t => t != arg))
                .Skip(1)
                .FirstOrDefault();
            
            return value != null;
        }

        /// <summary>
        /// Determines if the given argument matches the provided template.
        /// </summary>
        /// <param name="argument">Argument to evaluate</param>
        /// <param name="template">Template to match</param>
        /// <returns>Boolean</returns>
        public static bool IsTemplateMatch(string argument, string template)
        {
            return template.Split('|').Any(t => t == argument);
        }

        /// <summary>
        /// Splits a string into arguments, minding quotes and escaping.
        /// </summary>
        /// <param name="str">String to split</param>
        /// <returns>Arguments</returns>
        public static IEnumerable<string> SplitToArguments(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                yield break;

            var builder = new StringBuilder();
            var quoteScanned = false;
            var quoteOpened = false;

            foreach (var c in str)
            {
                switch (c)
                {
                    case '"' when quoteScanned:
                        quoteScanned = false;
                        builder.Append(c);
                        break;
                    
                    case '"':
                        quoteOpened = !quoteOpened;
                        quoteScanned = true;
                        break;
                    
                    case ' ' when !quoteOpened:
                        if (builder.Length > 0) yield return builder.ToString();
                        builder.Clear();
                        quoteScanned = false;
                        break;
                    
                    default:
                        quoteScanned = false;
                        builder.Append(c);
                        break;
                }
            }

            if (builder.Length > 0) yield return builder.ToString();
        }
    }
}