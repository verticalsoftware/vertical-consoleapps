using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Vertical.ConsoleApplications.Services
{
    public class StartupTaskBuilder
    {
        internal StartupTaskBuilder(IServiceCollection applicationServices)
        {
            ApplicationServices = applicationServices;

            ApplicationServices.TryAddSingleton(provider => new StartupTaskRunner(
                provider.GetServices<IStartupTask>()));
        }

        /// <summary>
        /// Gets the application services.
        /// </summary>
        public IServiceCollection ApplicationServices { get; }

        /// <summary>
        /// Registers a type used to perform asynchronous initialization.
        /// </summary>
        /// <typeparam name="T">Startup task type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public StartupTaskBuilder AddTask<T>() where T : class, IStartupTask
        {
            ApplicationServices.AddSingleton<IStartupTask, T>();
            return this;
        }

        /// <summary>
        /// Adds a delegate that is used to perform asynchronous initialization action.
        /// </summary>
        /// <param name="action">Delegate that handles the startup action</param>
        /// <returns>A reference to this instance</returns>
        public StartupTaskBuilder AddAction(Func<Task> action)
        {
            return AddAction(_ => action());
        }

        /// <summary>
        /// Adds a delegate that is used to perform asynchronous initialization action.
        /// </summary>
        /// <param name="action">Delegate that handles the startup action</param>
        /// <returns>A reference to this instance</returns>
        public StartupTaskBuilder AddAction(Func<IServiceProvider, Task> action)
        {
            ApplicationServices.AddSingleton<IStartupTask>(serviceProvider => new StartupTaskWrapper(
                serviceProvider,
                action));

            return this;
        }
    }
}