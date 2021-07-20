using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Commands
{
    internal static class HandlerDelegateFactory
    {
        internal static Func<object, IEnumerable<string>, CancellationToken, Task> Create(MethodInfo methodInfo)
        {
            var controllerType = methodInfo.DeclaringType ?? throw new InvalidOperationException();
            
            // Check parameters
            var parameters = methodInfo.GetParameters();

            if (!(parameters.Length == 2
                  && parameters[0].ParameterType == typeof(IEnumerable<string>)
                  && parameters[1].ParameterType == typeof(CancellationToken)))
            {
                var message =
                    $"Command handler {controllerType.Name}.{methodInfo.Name} has an incorrect parameter signature. " +
                    $"Expected ({nameof(IEnumerable<string>)}, {nameof(CancellationToken)}), but was " +
                    $"{(string.Join(",", parameters.Select(p => p.ParameterType.Name)))}";

                throw new InvalidOperationException(message);
            }
            
            // Check return type
            if (methodInfo.ReturnType != typeof(Task))
            {
                var message =
                    $"Command handler {controllerType.Name}.{methodInfo.Name} has the incorrect return type. " +
                    $"Expected {nameof(Task)} but was {methodInfo.ReturnType.Name}";

                throw new InvalidOperationException(message);
            }

            var targetParameter = Expression.Parameter(typeof(object));
            var argumentsParameter = Expression.Parameter(typeof(IEnumerable<string>));
            var cancelTokenParameter = Expression.Parameter(typeof(CancellationToken));
            var castExpression = Expression.Convert(targetParameter, controllerType);
            var handlerInvokeExpression = Expression.Call(castExpression,
                methodInfo,
                argumentsParameter,
                cancelTokenParameter);
            var lambdaExpression = Expression.Lambda<Func<object, IEnumerable<string>, CancellationToken, Task>>(
                handlerInvokeExpression,
                targetParameter,
                argumentsParameter,
                cancelTokenParameter);
            var asyncInvokeFunc = lambdaExpression.Compile();

            return asyncInvokeFunc;
        } 
    }
}