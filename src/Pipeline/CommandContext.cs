using System.Linq;

namespace Vertical.ConsoleApplications.Pipeline
{
    /// <summary>
    /// Defines the context of the command pipeline.
    /// </summary>
    public class CommandContext
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="arguments">Arguments</param>
        public CommandContext(string[] arguments)
        {
            Arguments = arguments;
        }

        /// <summary>
        /// Gets the context arguments.
        /// </summary>
        public string[] Arguments { get; }

        /// <inheritdoc />
        public override string ToString() => "[" + string.Join(", ", Arguments.Select(arg => $"\"{arg}\"")) + "]";
    }
}