using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Extensions;

namespace Vertical.ConsoleApplications.Routing
{
    internal static class RouteMetadataBuilder
    {
        internal static IEnumerable<Type> ScanForTypes(Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Where(type => type
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Any(method => method.GetCustomAttribute<CommandRouteAttribute>() != null));
        }
        
        internal static IEnumerable<RouteMetadata> BuildForType(Type type)
        {
            var hostRoute = type.GetCustomAttribute<CommandRouteAttribute>();
            var methods = type
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Select(method => (method, route: method.GetCustomAttribute<CommandRouteAttribute>()))
                .Where(t => t.route != null);

            foreach (var (method, route) in methods)
            {
                AssertHandlerParameters(method);
                AssertHandlerReturnType(method);

                yield return new RouteMetadata(hostRoute,
                    route!,
                    type,
                    method,
                    BuildFunction(method),
                    Arguments.Combine(hostRoute?.Route, route!.Route));
            }
        }

        private static Func<object,string[],CancellationToken,Task> BuildFunction(MethodInfo method)
        {
            var hostParameter = Expression.Parameter(typeof(object));
            var convertExpression = Expression.Convert(hostParameter, method.DeclaringType!);
            var argsParameter = Expression.Parameter(typeof(string[]));
            var cancelTokenParameter = Expression.Parameter(typeof(CancellationToken));
            var methodExpression = Expression.Call(convertExpression, method, argsParameter, cancelTokenParameter);
            var lambdaExpression = Expression.Lambda<Func<object, string[], CancellationToken, Task>>(
                methodExpression,
                hostParameter,
                argsParameter,
                cancelTokenParameter);
            
            return lambdaExpression.Compile();
        }

        private static void AssertHandlerReturnType(MethodInfo method)
        {
            var parameters = method.GetParameters();

            if (parameters.Length == 2
                && parameters[0].ParameterType == typeof(string[])
                && parameters[1].ParameterType == typeof(CancellationToken))
            {
                return;
            }

            var message =
                $"Command route handler {method.DeclaringType!.Name}.{method.Name} does not "
                + "have the expected parameter signature."
                + Environment.NewLine
                + $"\tExpected: ({typeof(string[])}, {typeof(CancellationToken)})"
                + Environment.NewLine
                + $"\tBut was:  ({string.Join(", ", parameters.Select(p => p.GetType()))})";

            throw new InvalidOperationException(message);
        }

        private static void AssertHandlerParameters(MethodInfo method)
        {
            if (method.ReturnType == typeof(Task))
                return;
            
            var message =
                $"Command route handler {method.DeclaringType!.Name}.{method.Name} does not "
                + "have the expected return type signature."
                + Environment.NewLine
                + $"\tExpected: {typeof(Task)}"
                + Environment.NewLine
                + $"\tBut was:  {method.ReturnType}";

            throw new InvalidOperationException(message);
        }
    }
}