using System;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Providers
{
    /// <summary>
    /// Represents an object that provides arguments inputs to the console application.
    /// </summary>
    public interface IArgumentProvider
    {
        /// <summary>
        /// Invokes the provided function for each set of inputs provided by this instance.
        /// </summary>
        /// <param name="invoke">An invocation function.</param>
        /// <returns>Task</returns>
        Task ExecuteInvocationsAsync(Func<string[], Task> invoke);
    }
}