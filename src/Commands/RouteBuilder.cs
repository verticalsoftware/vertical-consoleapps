using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Vertical.ConsoleApplications.Commands
{
    internal static class RouteBuilder
    {
        /// <summary>
        /// Builds route maps for the given controller type.
        /// </summary>
        /// <param name="controllerType">Controller type</param>
        /// <returns><see cref="IEnumerable{T}"/></returns>
        internal static IEnumerable<KeyValuePair<string, RouteMetadata>> BuildRouteMaps(Type controllerType)
        {
            var baseTemplate = controllerType
                .GetCustomAttribute<CommandAttribute>()?
                .Template
                .Trim() ?? null;

            const BindingFlags methodBindingFlags = BindingFlags.Public | BindingFlags.Instance;

            // Find all methods decorated with CommandAttribute
            var routeCandidates = controllerType
                .GetMethods(methodBindingFlags)
                .Select(method => (method, command: method.GetCustomAttribute<CommandAttribute>()))
                .Where(item => item.command != null);

            foreach (var (method, command) in routeCandidates)
            {
                var template = TemplateBuilder.Combine(baseTemplate, command!.Template);
                var asyncHandler = HandlerDelegateFactory.Create(method);
                var routeMetadata = new RouteMetadata(controllerType, method, asyncHandler);

                yield return new KeyValuePair<string, RouteMetadata>(template, routeMetadata);
            }
        }
    }
}