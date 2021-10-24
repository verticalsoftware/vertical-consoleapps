namespace Vertical.ConsoleApplications.Pipeline
{
    /// <summary>
    /// Represents an object that constructs a delegate that can be used
    /// to invoke a middleware pipeline.
    /// </summary>
    public interface IMiddlewareFactory
    {
        /// <summary>
        /// Creates a delegate that can be used to invoke a middleware pipeline
        /// with context.
        /// </summary>
        /// <returns><see cref="PipelineDelegate"/></returns>
        PipelineDelegate Create();
    }
}