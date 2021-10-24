using System.Threading;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Pipeline
{
    /// <summary>
    /// Handles invocations of pipeline middleware.
    /// </summary>
    public delegate Task PipelineDelegate(CommandContext context, CancellationToken cancellationToken);
}