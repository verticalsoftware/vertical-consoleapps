using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Providers
{
    /// <summary>
    /// Represents an object that provides commands to the pipeline.
    /// </summary>
    public interface ICommandProvider
    {
        /// <summary>
        /// Executes commands from the provider.
        /// </summary>
        /// <param name="asyncInvoke">A function that is invoked per command set.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task</returns>
        Task ExecuteCommandsAsync(Func<string[], Task> asyncInvoke, 
            CancellationToken cancellationToken);
    }
}