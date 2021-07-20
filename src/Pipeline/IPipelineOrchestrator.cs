using System.Threading;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Pipeline
{
    public interface IPipelineOrchestrator
    {
        /// <summary>
        /// Executes the arguments pipeline.
        /// </summary>
        /// <param name="arguments">Arguments</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        Task ExecutePipelineAsync(string[] arguments, CancellationToken cancellationToken);
    }
}