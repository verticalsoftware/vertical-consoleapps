using System.Threading;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Pipeline
{
    /// <summary>
    /// Represents a discrete task in a middleware pipeline.
    /// </summary>
    public interface IMiddleware
    {
        /// <summary>
        /// When implemented by a class, performs the logic of the middleware.
        /// </summary>
        /// <param name="context">Context that is shared in the middleware pipeline</param>
        /// <param name="next">A delegate that transfers control to the next component in the pipeline</param>
        /// <param name="cancellationToken">A token that can be observed for cancellation requests</param>
        /// <returns>Task</returns>
        Task InvokeAsync(RequestContext context, PipelineDelegate next, CancellationToken cancellationToken);
    }
}