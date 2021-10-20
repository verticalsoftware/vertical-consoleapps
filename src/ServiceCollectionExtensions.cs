using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Vertical.ConsoleApplications.Routing;

namespace Vertical.ConsoleApplications
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a command handler.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="type">Handler implementation type</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddCommandHandler(
            this IServiceCollection services,
            Type type)
        {
            return services.AddHandlerDescriptor(HandlerDescriptor.FromType(type));
        }

        /// <summary>
        /// Adds a command handler.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="command">The command to associate with the handler</param>
        /// <param name="type">Handler implementation type</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddCommandHandler(
            this IServiceCollection services,
            string command,
            Type type)
        {
            return services.AddHandlerDescriptor(new HandlerDescriptor(type, command));
        }

        /// <summary>
        /// Adds a command handler.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <typeparam name="T">Handler implementation type</typeparam>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddCommandHandler<T>(this IServiceCollection services)
            where T : ICommandHandler
        {
            return services.AddCommandHandler(typeof(T));
        }

        /// <summary>
        /// Adds a command handler.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="command">The command to associate with the handler</param>
        /// <typeparam name="T">Handler implementation type</typeparam>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddCommandHandler<T>(this IServiceCollection services, string command)
            where T : ICommandHandler
        {
            return services.AddCommandHandler(command, typeof(T));
        }

        /// <summary>
        /// Adds all handlers found in the exported types of an assembly.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="assembly">
        /// The assembly to scan. If this parameter is null, the calling assembly
        /// is scanned.
        /// </param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddCommandHandlers(this IServiceCollection services,
            Assembly? assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            var types = assembly
                .ExportedTypes
                .Where(type => type.IsPublic
                               && !type.IsAbstract
                               && !type.IsInterface
                               && typeof(ICommandHandler).IsAssignableFrom(type))
                .ToArray();

            foreach (var type in types)
            {
                services.AddCommandHandler(type);
            }

            return services;
        }

        private static IServiceCollection AddHandlerDescriptor(
            this IServiceCollection services,
            HandlerDescriptor descriptor)
        {
            services.TryAddSingleton(provider =>
            {
                var descriptors = provider.GetServices<HandlerDescriptor>();
                
                return descriptors.ToDictionary(
                    dsc => dsc.Command,
                    dsc => new Func<IServiceProvider, ICommandHandler>(sp => (ICommandHandler)
                        sp.GetRequiredService(dsc.ImplementationType)));
            });
            services.TryAddSingleton<ICommandRouter, CommandRouter>();
            services.AddSingleton(descriptor);
            services.AddScoped(descriptor.ImplementationType);

            return services;
        }
    }
}