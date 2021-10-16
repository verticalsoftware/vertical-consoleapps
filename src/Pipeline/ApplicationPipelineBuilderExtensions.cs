using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Vertical.ConsoleApplications.Pipeline
{
    public static class ApplicationPipelineBuilderExtensions
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
    }
}