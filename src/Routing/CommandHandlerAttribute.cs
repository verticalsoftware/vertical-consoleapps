using System;

namespace Vertical.ConsoleApplications.Routing
{
    /// <summary>
    /// Applied to instances of classes that handle commands.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandHandlerAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="command">Command this type handles.</param>
        public CommandHandlerAttribute(string command)
        {
            Command = command;
        }
        
        /// <summary>
        /// Gets the command the implementation handles.
        /// </summary>
        public string Command { get; }

        /// <inheritdoc />
        public override string ToString() => Command;
    }
}