using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Routing;

namespace Vertical.ConsoleApplications.Pipeline
{
    public static class BuilderExtensions
    {
        /// <summary>
        /// Inserts middleware to the argument pipeline that listens
        /// for an exit command.
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <param name="commands">One or more commands used to signal application stop.</param>
        /// <returns>A reference to the pipeline builder.</returns>
        /// <remarks>
        /// This method will default to using the "exit" command if none are specified.
        /// </remarks>
        public static ApplicationPipelineBuilder UseExitCommand(
            this ApplicationPipelineBuilder builder,
            params string[] commands)
        {
            if (commands.Length == 0)
            {
                commands = new[] { "exit" };
            }
            
            builder.UseMiddleware<ExitCommandMiddleware>(new object?[]
            {
                commands,
                builder.ServiceProvider.GetService<ILogger<ExitCommandMiddleware>>()
            });

            return builder;
        }

        /// <summary>
        /// Inserts middleware to the argument pipeline that replaces tokens in arguments
        /// with environment variable values.
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <returns>A reference to this pipeline builder.</returns>
        public static ApplicationPipelineBuilder UseEnvironmentVariables(
            this ApplicationPipelineBuilder builder)
        {
            return builder.UseTokenReplacement(
                "environment variable",
                arg => Regex.Replace(arg, @"(?<!\\)\$(\w+)", match => Environment.GetEnvironmentVariable(
                    match.Groups[1].Value) ?? match.Value));
        }

        /// <summary>
        /// Inserts middleware to the argument pipeline that replaces tokens in arguments
        /// with <see cref="Environment.SpecialFolder"/> values.
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <returns>A reference to this pipeline builder.</returns>
        public static ApplicationPipelineBuilder UseSpecialFolders(
            this ApplicationPipelineBuilder builder)
        {
            return builder.UseTokenReplacement(
                "special folders",
                arg => Regex.Replace(arg, @"(?<!\\)\$(\w+)", match =>
                {
                    var value = match.Groups[1].Value;
                    
                    return (Enum.TryParse(value, out Environment.SpecialFolder specialFolder))
                        ? Environment.GetFolderPath(specialFolder)
                        : match.Value;
                }));
        }

        /// <summary>
        /// Inserts middleware to the argument pipeline that replaces specific argument
        /// values.
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <param name="token">The argument to match.</param>
        /// <param name="replaceValue">The value to replace as the argument value.</param>
        /// <returns>A reference to this pipeline builder.</returns>
        public static ApplicationPipelineBuilder UseReplacementToken(
            this ApplicationPipelineBuilder builder,
            string token,
            Func<string> replaceValue)
        {
            return builder.UseTokenReplacement(
                "token substitution",
                arg => arg == token ? replaceValue() : arg);
        }

        /// <summary>
        /// Inserts middleware to the argument pipeline that replaces argument values
        /// using a function delegate.
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <param name="replaceFunction">
        /// A function that receives an argument and returns its replacement value.
        /// </param>
        /// <returns>A reference to the pipeline builder</returns>
        public static ApplicationPipelineBuilder UseReplacementFunction(
            this ApplicationPipelineBuilder builder,
            Func<string, string> replaceFunction)
        {
            return builder.UseTokenReplacement("custom replacement", replaceFunction);
        }

        /// <summary>
        /// Inserts middleware to the argument pipeline that uses delegates to handle
        /// command implementations.
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <param name="configure">
        /// An delegate used to construct route handlers for the application.
        /// </param>
        /// <returns>A reference to the pipeline builder</returns>
        public static ApplicationPipelineBuilder UseCommands(
            this ApplicationPipelineBuilder builder,
            Action<CommandRoutingBuilder> configure)
        {
            using var routingBuilder = new CommandRoutingBuilder(builder);

            configure(routingBuilder);
            
            return builder;
        }

        private static ApplicationPipelineBuilder UseTokenReplacement(
            this ApplicationPipelineBuilder builder,
            string context,
            Func<string, string> replaceFunction)
        {
            builder.UseMiddleware<TokenReplacementMiddleware>(
                context,
                replaceFunction,
                builder.ServiceProvider.GetService<ILogger<TokenReplacementMiddleware>>());
            
            return builder;
        }
    }
}