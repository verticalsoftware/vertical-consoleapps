using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Vertical.ConsoleApplications.Pipeline;

namespace Vertical.ConsoleApplications.Routing
{
    public class RoutingBuilder
    {
        private static readonly ConstructorInfo ControllerConstructorInfo =
            typeof(ControllerHandler<>)
                .GetConstructors()
                .First(ctor => ctor.GetParameters().Length == 2);
        
        internal RoutingBuilder(IServiceCollection applicationServices)
        {
            ApplicationServices = applicationServices;

            applicationServices.AddSingleton(serviceProvider => new HandlerMap(serviceProvider
                .GetServices<RouteDescriptor>()));
        }

        /// <summary>
        /// Gets the application's service collection.
        /// </summary>
        public IServiceCollection ApplicationServices { get; }

        /// <summary>
        /// Maps a command route to a handler implementation.
        /// </summary>
        /// <param name="route">Route</param>
        /// <param name="handler">
        /// A delegate that implements the command logic
        /// </param>
        /// <returns>A reference to this instance</returns>
        public RoutingBuilder Map(string route, Func<CommandContext, CancellationToken, Task> handler)
        {
            ApplicationServices.AddSingleton(new RouteDescriptor(route, _ => new CommandHandlerWrapper(handler)));
            return this;
        }

        /// <summary>
        /// Maps a <see cref="ICommandHandler"/> implementation for the given route.
        /// </summary>
        /// <param name="route">The route to map.</param>
        /// <param name="implementationFactory">Factory delegate that creates the handler instance.</param>
        /// <returns>A reference to this instance.</returns>
        public RoutingBuilder MapHandler(string route, Func<IServiceProvider, ICommandHandler> implementationFactory)
        {
            ApplicationServices.AddScoped(_ => new RouteDescriptor(route, implementationFactory));
            return this;
        }

        /// <summary>
        /// Maps a <see cref="ICommandHandler"/> implementation for the given route.
        /// </summary>
        /// <param name="route">The route to map</param>
        /// <typeparam name="T">The command handler type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public RoutingBuilder MapHandler<T>(string route) where T : class, ICommandHandler
        {
            ApplicationServices
                .AddScoped<T>()
                .AddSingleton(new RouteDescriptor(route, serviceProvider =>
                serviceProvider.GetRequiredService<T>()));

            return this;
        }

        /// <summary>
        /// Maps the route or routes in a command controller implementation.
        /// </summary>
        /// <param name="type">Controller type</param>
        /// <returns>A reference to this instance.</returns>        
        public RoutingBuilder MapController(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            
            var methods = type
                .GetMethods()
                .Select(methodInfo => new
                {
                    method = methodInfo,
                    command = methodInfo.GetCustomAttribute<CommandAttribute>()
                })
                .Where(item =>
                {
                    var method = item.method;
                    
                    if (item.command == null)
                        return false;
                    
                    if (method.ReturnType != typeof(Task))
                        return false;

                    var parameters = method.GetParameters();

                    if (parameters.Length != 2)
                        return false;

                    if (parameters[0].ParameterType != typeof(CommandContext))
                        return false;

                    return parameters[1].ParameterType == typeof(CancellationToken);
                })
                .ToArray();

            if (methods.Length == 0)
                return this;

            ApplicationServices.AddScoped(type);

            foreach (var methodInfo in methods)
            {
                // Create handler delegate
                var controller = Expression.Parameter(type);
                var context = Expression.Parameter(typeof(CommandContext));
                var cancelToken = Expression.Parameter(typeof(CancellationToken));
                var handle = Expression.Call(controller, methodInfo.method, context, cancelToken);
                var lambda = Expression.Lambda(
                    handle,
                    controller,
                    context,
                    cancelToken);
                
                // Create an implementation factory
                var serviceProvider = Expression.Parameter(typeof(IServiceProvider));
                var handlerType = typeof(ControllerHandler<>).MakeGenericType(type);
                var getController = Expression.Convert( 
                    Expression.Call(
                    serviceProvider,
                    typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetService))!,
                    Expression.Constant(type)),
                    type);
                var constructor = handlerType
                    .GetConstructors()
                    .First(ctor => ctor.GetParameters().Length == 2);
                var activator = Expression.New(
                    constructor,
                    getController,
                    lambda);
                var factory = Expression.Lambda<Func<IServiceProvider, ICommandHandler>>(
                        activator,
                        serviceProvider)
                    .Compile();

                MapHandler(methodInfo.command!.Route, sp => factory(sp));
            }

            return this;
        }

        /// <summary>
        /// Maps the route or routes in a command controller implementation.
        /// </summary>
        /// <typeparam name="T">Controller type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public RoutingBuilder MapController<T>() where T : class => MapController(typeof(T));
    }
}