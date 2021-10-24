using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Middleware;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;
using Vertical.ConsoleApplications.Utilities;

namespace Vertical.ConsoleApplications
{
    public static class PipelineBuilderExtensions
    {
        /// <summary>
        /// Inserts middleware that listens for a predefined argument, and when matched
        /// signals the application to stop.
        /// </summary>
        /// <param name="pipelineBuilder">Pipeline builder</param>
        /// <param name="exitCommand">Exit command to listen for</param>
        /// <returns>A reference to the given pipeline builder</returns>
        public static IPipelineBuilder UseExitCommand(
            this IPipelineBuilder pipelineBuilder,
            string exitCommand)
        {
            return pipelineBuilder.UseExitCommands(new[] { exitCommand });
        }
        
        /// <summary>
        /// Inserts middleware that listens for a predefined set of arguments,
        /// and when matched signals the application to stop.
        /// </summary>
        /// <param name="pipelineBuilder">Pipeline builder</param>
        /// <param name="exitCommands">Exit commands to listen for</param>
        /// <returns>A reference to the given pipeline builder</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exitCommands"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="exitCommands"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="exitCommands"/> array contains one or more elements that are null or only whitespace.
        /// </exception>
        public static IPipelineBuilder UseExitCommands(
            this IPipelineBuilder pipelineBuilder,
            string[] exitCommands)
        {
            if (exitCommands == null)
            {
                throw new ArgumentNullException(nameof(exitCommands));
            }

            if (exitCommands.Length == 0)
            {
                throw new ArgumentException("A minimum of one exit command must be specified.", nameof(exitCommands));
            }

            if (exitCommands.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("Null or whitespace strings not allowed.", nameof(exitCommands));
            }
            
            return pipelineBuilder.UseMiddleware(serviceProvider =>
                new ExitCommandMiddleware(
                    exitCommands,
                    serviceProvider.GetRequiredService<IHostApplicationLifetime>(),
                    serviceProvider.GetService<ILogger<ExitCommandMiddleware>>()));
        }

        /// <summary>
        /// Inserts middleware that replaces a specific argument with a value.
        /// </summary>
        /// <param name="pipelineBuilder">Pipeline builder</param>
        /// <param name="argument">Argument to match</param>
        /// <param name="replacementValue">Replacement value</param>
        /// <returns>A reference to the given pipeline builder</returns>
        public static IPipelineBuilder UseArgumentReplacement(
            this IPipelineBuilder pipelineBuilder,
            string argument,
            string replacementValue)
        {
            return pipelineBuilder.UseArgumentReplacement(arg => arg == argument ? replacementValue : arg);
        }

        /// <summary>
        /// Inserts middleware that replaces arguments with other values.
        /// </summary>
        /// <param name="pipelineBuilder">Pipeline builder</param>
        /// <param name="replaceFunction">
        /// A function that receives the original argument and returns the value to replace
        /// it with.
        /// </param>
        /// <returns>Pipeline builder</returns>
        public static IPipelineBuilder UseArgumentReplacement(
            this IPipelineBuilder pipelineBuilder,
            Func<string, string> replaceFunction)
        {
            return pipelineBuilder.UseMiddleware(sp => new ArgumentReplacementMiddleware(
                    replaceFunction,
                    sp.GetService<ILogger<ArgumentReplacementMiddleware>>()));
        }

        /// <summary>
        /// Inserts middleware that replaces tokens in argument variables with environment
        /// variable values.
        /// </summary>
        /// <param name="pipelineBuilder">Pipeline builder</param>
        /// <returns>A reference to the given pipeline builder</returns>
        public static IPipelineBuilder UseEnvironmentVariableTokens(this IPipelineBuilder pipelineBuilder)
        {
            return pipelineBuilder.UseArgumentReplacement(ArgumentHelpers.ReplaceEnvironmentVariables);
        }

        /// <summary>
        /// Inserts middleware that replaces tokens in argument variables with special folder
        /// values.
        /// </summary>
        /// <param name="pipelineBuilder">Pipeline builder</param>
        /// <returns>A reference to the given pipeline builder</returns>
        public static IPipelineBuilder UseSpecialFolderTokens(this IPipelineBuilder pipelineBuilder)
        {
            return pipelineBuilder.UseArgumentReplacement(ArgumentHelpers.ReplaceSpecialFolderPaths);
        }

        /// <summary>
        /// Inserts middleware that routes action to a pre-defined implementation handler.
        /// </summary>
        /// <param name="pipelineBuilder">Pipeline builder</param>
        /// <param name="configureRouting">
        /// A delegate that can be used to configure routes.
        /// </param>
        /// <returns>A reference to the given pipeline builder</returns>
        public static IPipelineBuilder UseRouting(
            this IPipelineBuilder pipelineBuilder,
            Action<RoutingBuilder> configureRouting)
        {
            configureRouting(new RoutingBuilder(pipelineBuilder.ApplicationServices));
            return pipelineBuilder.UseMiddleware<CommandRoutingMiddleware>();
        }
    }
}