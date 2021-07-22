using System.Threading;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Pipeline
{
    public interface IPipelineOrchestrator
    {
        Task ExecutePipelineAsync(string[] args, CancellationToken cancellationToken);
    }
}