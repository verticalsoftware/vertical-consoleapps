namespace Vertical.ConsoleApplications.Pipeline
{
    /// <summary>
    /// Performs initialization on request objects.
    /// </summary>
    public interface IRequestInitializer
    {
        /// <summary>
        /// When implemented by a class, initializes request data.
        /// </summary>
        /// <param name="context">Context to initialize</param>
        void Initialize(RequestContext context);
    }
}