using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Services
{
    /// <summary>
    /// Represents the interface of an implementation that performs a startup task.
    /// </summary>
    public interface IStartupTask
    {
        /// <summary>
        /// When implemented by a class, performs initialization work.
        /// </summary>
        /// <param name="args">Application entry arguments</param>
        /// <returns>Task that completes when the initialization work is finished.</returns>
        Task InitializeAsync(string[] args);
    }
}