using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Services
{
    /// <summary>
    /// Represents a service that executes startup tasks asynchronously.
    /// </summary>
    public interface IStartupTaskRunner
    {
        /// <summary>
        /// Executes startup tasks.
        /// </summary>
        Task RunStartupTasksAsync();
    }
}