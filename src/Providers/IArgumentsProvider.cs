using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Providers
{
    /// <summary>
    /// Represents the interface of an object that provides arguments to an
    /// execution pipeline.
    /// </summary>
    public interface IArgumentsProvider
    {
        /// <summary>
        /// When implemented by a class, leverages the provided asynchronous delegate
        /// to invoke its arguments.
        /// </summary>
        /// <param name="handler">
        /// An asynchronous function that executes a pipeline using a supplied set of
        /// arguments, and returns a Task that completes with the pipeline.
        /// </param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>Task</returns>
        Task InvokeArgumentsAsync(Func<IReadOnlyList<string>, CancellationToken, Task> handler,
            CancellationToken cancellationToken);
    }
}