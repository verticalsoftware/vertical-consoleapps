using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Services
{
    internal class StartupTaskRunner : IStartupTaskRunner
    {
        private readonly IEnumerable<IStartupTask> _startupTasks;
        private readonly string[] _entryArguments;

        internal StartupTaskRunner(string[] entryArguments, IEnumerable<IStartupTask> startupTasks)
        {
            _entryArguments = entryArguments;
            _startupTasks = startupTasks;
        }

        /// <summary>
        /// Executes startup tasks.
        /// </summary>
        public async Task RunStartupTasksAsync()
        {
            var args = _entryArguments;
            
            foreach (var startupTask in _startupTasks)
            {
                await startupTask.InitializeAsync(args);
            }
        }
    }
}