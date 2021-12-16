using System;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Services
{
    internal class StartupTaskWrapper : IStartupTask
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Func<IServiceProvider, Task> _implementation;

        internal StartupTaskWrapper(IServiceProvider serviceProvider, Func<IServiceProvider, Task> implementation)
        {
            _serviceProvider = serviceProvider;
            _implementation = implementation ?? throw new ArgumentNullException(nameof(implementation));
        }

        /// <inheritdoc />
        public Task InitializeAsync() => _implementation(_serviceProvider);
    }
}