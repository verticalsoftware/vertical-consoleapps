using System;

namespace Vertical.ConsoleApplications.Commands
{
    /// <summary>
    /// Metadata attribute that designates a class or method a path for a specific command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="template">Template that defines routing to a handler.</param>
        /// <param name="description">Optional description of the command</param>
        public CommandAttribute(string template, string? description = null)
        {
            Template = template?.Trim() ?? throw new ArgumentNullException(nameof(template));
            Description = description;

            if (template.Length == 0)
            {
                throw new ArgumentException("Template cannot be empty or whitespace", nameof(template));
            }
        }

        /// <summary>
        /// Gets the template that defines routing to a handler.
        /// </summary>
        public string Template { get; }

        /// <summary>
        /// Gets the command description.
        /// </summary>
        public string? Description { get; }

        /// <inheritdoc />
        public override string ToString() => Template;
    }
}