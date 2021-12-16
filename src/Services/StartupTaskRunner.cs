using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Services
{
    internal class StartupTaskRunner : IStartupTaskRunner
    {
        private readonly IEnumerable<IStartupTask> _startupTasks;

        internal StartupTaskRunner(IEnumerable<IStartupTask> startupTasks)
        {
            _startupTasks = startupTasks;
        }

        /// <summary>
        /// Executes startup tasks.
        /// </summary>
        public async Task RunStartupTasksAsync()
        {
            foreach (var startupTask in _startupTasks)
            {
                await startupTask.InitializeAsync();
            }
        }
    }
}