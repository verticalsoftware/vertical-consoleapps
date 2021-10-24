namespace Vertical.ConsoleApplications.Pipeline
{
    /// <summary>
    /// Represents the context available to the argument pipeline.
    /// </summary>
    public class CommandContext
    {
        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="arguments">Context arguments</param>
        /// <param name="data">Gets additional data associated with the context</param>
        public CommandContext(string[] arguments, object? data)
        {
            Arguments = arguments;
            Data = data;
            OriginalFormat = string.Join(' ', arguments);
        }
        
        /// <summary>
        /// Gets the context arguments.
        /// </summary>
        public string[] Arguments { get; }
        
        /// <summary>
        /// Gets the original format.
        /// </summary>
        public string OriginalFormat { get; }

        /// <summary>
        /// Gets application defined data associated with the context.
        /// </summary>
        public object? Data { get; }

        /// <inheritdoc />
        public override string ToString() => OriginalFormat;
    }
}