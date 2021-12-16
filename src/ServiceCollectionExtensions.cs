using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;
using Vertical.ConsoleApplications.Services;

namespace Vertical.ConsoleApplications
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds command routing services.
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <returns>A reference to the given service collection</returns>
        public static IServiceCollection AddCommandRouting(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSingleton<ICommandRouter, CommandRouter>();
        }

        /// <summary>
        /// Adds a services that performs request initialization.
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <typeparam name="T"><see cref="IRequestInitializer"/> implementation type</typeparam>
        /// <returns>A reference to the given service collection</returns>
        public static IServiceCollection AddRequestInitializer<T>(this IServiceCollection serviceCollection)
            where T : class, IRequestInitializer => serviceCollection.AddSingleton<IRequestInitializer, T>();

        /// <summary>
        /// Adds a services that performs request initialization.
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <param name="implementationFactory">A function that creates the instance.</param>
        /// <typeparam name="T"><see cref="IRequestInitializer"/> implementation type</typeparam>
        /// <returns>A reference to the given service collection</returns>
        public static IServiceCollection AddRequestInitializer<T>(
            this IServiceCollection serviceCollection,
            Func<IServiceProvider, T> implementationFactory)
            where T : class, IRequestInitializer
        {
            return serviceCollection.AddSingleton<IRequestInitializer>(implementationFactory);
        }

        /// <summary>
        /// Adds a service that performs request initialization using a delegate.
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <param name="initializeAction">An action that performs initialization of a request context</param>
        /// <returns>A reference to the given service collection</returns>
        public static IServiceCollection AddRequestInitializer(this IServiceCollection serviceCollection,
            Action<RequestContext> initializeAction) => serviceCollection.AddRequestInitializer(_ =>
                new RequestInitializingWrapper(initializeAction));

        /// <summary>
        /// Adds services that orchestrate the asynchronous execution a series of tasks that
        /// happen when the host starts up.
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <param name="configureStartup">An action that performs the configuration of startup tasks</param>
        /// <returns>A reference to the given service collection</returns>
        public static IServiceCollection AddStartupTasks(this IServiceCollection serviceCollection,
            Action<StartupTaskBuilder> configureStartup)
        {
            configureStartup(new StartupTaskBuilder(serviceCollection));
            return serviceCollection;
        }
    }
}