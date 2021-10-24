using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;

namespace Vertical.ConsoleApplications
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a scoped context factory.
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <typeparam name="T">Type of <see cref="IContextDataFactory"/></typeparam>
        /// <returns>A reference to the given service collection</returns>
        public static IServiceCollection AddContextFactory<T>(this IServiceCollection serviceCollection)
            where T : class, IContextDataFactory
        {
            serviceCollection.AddScoped<IContextDataFactory, T>();
            return serviceCollection;
        }

        /// <summary>
        /// Adds a scoped context factory.
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <param name="implementationFactory">Factory method that creates the command context</param>
        /// <typeparam name="T">Type of <see cref="IContextDataFactory"/></typeparam>
        /// <returns>A reference to the given service collection</returns>
        public static IServiceCollection AddContextFactory<T>(
            this IServiceCollection serviceCollection,
            Func<IServiceProvider, T> implementationFactory)
            where T : IContextDataFactory
        {
            serviceCollection.AddScoped<IContextDataFactory>(sp => implementationFactory(sp));
            return serviceCollection;
        }

        /// <summary>
        /// Adds a delegate used to create the <see cref="CommandContext"/> for each
        /// request.
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <param name="factory">
        /// A delegate function that receives the arguments and returns the <see cref="CommandContext"/>
        /// to introduce to the pipeline.
        /// </param>
        /// <returns>A reference to the given service collection</returns>
        public static IServiceCollection AddContextFactory(
            this IServiceCollection serviceCollection,
            Func<string[], object?> factory)
        {
            return serviceCollection.AddScoped<IContextDataFactory>(_ => new ContextDataFactoryWrapper(factory));
        }

        /// <summary>
        /// Adds command routing services.
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <returns>A reference to the given service collection</returns>
        public static IServiceCollection AddCommandRouting(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSingleton<ICommandRouter, CommandRouter>();
        }
    }
}