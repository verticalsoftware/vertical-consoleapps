using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Vertical.ConsoleApplications.Middleware;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.Pipelines.DependencyInjection;

namespace Vertical.ConsoleApplications.Routing
{
    public class RoutingBuilder
    {
        internal RoutingBuilder(IPipelineBuilder<RequestContext> applicationPipeline)
        {
            ApplicationPipeline = applicationPipeline;
            ApplicationServices = applicationPipeline.ApplicationServices;

            ApplicationServices.AddSingleton(serviceProvider => new HandlerMap(serviceProvider
                .GetServices<RouteDescriptor>()));
        }

        /// <summary>
        /// Gets the application pipeline.
        /// </summary>
        public IPipelineBuilder<RequestContext> ApplicationPipeline { get; }

        /// <summary>
        /// Gets the application's service collection.
        /// </summary>
        public IServiceCollection ApplicationServices { get; }

        /// <summary>
        /// Registers a delegate that can be used to handle routes that aren't matched.
        /// </summary>
        /// <param name="action">An action that receives the unmatched context.</param>
        /// <returns>A reference to this instance.</returns>
        public RoutingBuilder MapUnmatched(Action<RequestContext> action)
        {
            ApplicationPipeline.UseMiddleware(_ => new UnmatchedRouteMiddleware(action));
            return this;
        }

        /// <summary>
        /// Maps a command route to a handler implementation.
        /// </summary>
        /// <param name="route">Route</param>
        /// <param name="handler">
        /// A delegate that implements the command logic
        /// </param>
        /// <returns>A reference to this instance</returns>
        public RoutingBuilder Map(string route, Func<RequestContext, CancellationToken, Task> handler)
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
            return MapDescriptor(route, typeof(T));
        }

        /// <summary>
        /// Maps a <see cref="ICommandHandler"/> implementation for the given route.
        /// </summary>
        /// <param name="route">The route to map</param>
        /// <param name="type">The command handler type</param>
        /// <returns>A reference to this instance.</returns>
        public RoutingBuilder MapHandler(string route, Type type)
        {
            if (!typeof(ICommandHandler).IsAssignableFrom(type))
            {
                throw new ArgumentException($"Type '{type}' is not assignable to {typeof(ICommandHandler)}");
            }

            return MapDescriptor(route, type);
        }
        
        /// <summary>
        /// Maps the route or routes in a type that have one or more command
        /// handler implementations.
        /// </summary>
        /// <typeparam name="T">Controller-service type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public RoutingBuilder MapHandler<T>() where T : class => MapHandler(typeof(T));

        /// <summary>
        /// Maps the route or routes in a type that have one or more command
        /// handler implementations.
        /// </summary>
        /// <param name="type">Controller-service type</param>
        /// <returns>A reference to this instance.</returns>        
        public RoutingBuilder MapHandler(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (typeof(ICommandHandler).IsAssignableFrom(type))
            {
                var commandAttribute = type.GetCustomAttribute<CommandAttribute>();
                if (commandAttribute != null)
                {
                    // Faster than method mapping
                    MapHandler(commandAttribute.Route, type);
                    return this;
                }
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

                    var parameters = method.GetParameters();

                    var isSignatureMatch = parameters.Length == 2
                                           && parameters[0].ParameterType == typeof(RequestContext)
                                           && parameters[1].ParameterType == typeof(CancellationToken)
                                           && !parameters[0].ParameterType.IsByRef
                                           && !parameters[1].ParameterType.IsByRef
                                           && method.ReturnType == typeof(Task);

                    if (isSignatureMatch) 
                        return true;
                    
                    var message = $"Method {type}.{method} is decorated to handle command '{item.command}' "
                                  + "but its signature is not compatible";
                    
                    throw new InvalidOperationException(message);
                })
                .ToArray();

            if (methods.Length == 0)
                return this;

            ApplicationServices.AddScoped(type);

            foreach (var methodInfo in methods)
            {
                // Create handler delegate
                var controller = Expression.Parameter(type);
                var context = Expression.Parameter(typeof(RequestContext));
                var cancelToken = Expression.Parameter(typeof(CancellationToken));
                var handle = Expression.Call(controller, methodInfo.method, context, cancelToken);
                var lambda = Expression.Lambda(
                    handle,
                    controller,
                    context,
                    cancelToken);
                
                // Create an implementation factory
                var serviceProvider = Expression.Parameter(typeof(IServiceProvider));
                var handlerType = typeof(DecoratedMethodHandler<>).MakeGenericType(type);
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
        /// Maps all handlers types found in an assembly.
        /// </summary>
        /// <param name="assembly">
        /// The assembly that contains implementations to map. If <code>null</code> is given
        /// then the calling assembly's types are scanned.
        /// </param>
        /// <returns><see cref="RoutingBuilder"/></returns>
        public RoutingBuilder MapHandlers(Assembly? assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            var handlerTypes = assembly
                .ExportedTypes
                .Where(type => type.IsClass
                               && !type.IsInterface
                               && !type.IsAbstract
                               && (type.GetCustomAttribute<CommandHandlerAttribute>() != null
                               || typeof(ICommandHandler).IsAssignableFrom(type)));

            foreach (var type in handlerTypes)
            {
                MapHandler(type);
            }

            return this;
        }

        private RoutingBuilder MapDescriptor(string route, Type type)
        {
            ApplicationServices
                .AddScoped(type)
                .AddSingleton(new RouteDescriptor(route, serviceProvider => (ICommandHandler)
                    serviceProvider.GetRequiredService(type)));
            
            return this;
        }
    }
}