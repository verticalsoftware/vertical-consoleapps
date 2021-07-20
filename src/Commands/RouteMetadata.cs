using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Commands
{
    /// <summary>
    /// Defines the metadata of a command route.
    /// </summary>
    public class RouteMetadata
    {
        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="controllerType">The controller type.</param>
        /// <param name="methodInfo">The method that is invoked on a controller instance.</param>
        /// <param name="asyncHandler">The asynchronous handler function.</param>
        public RouteMetadata(
            Type controllerType,
            MethodInfo methodInfo,
            Func<object, IEnumerable<string>, CancellationToken, Task> asyncHandler)
        {
            ControllerType = controllerType;
            MethodInfo = methodInfo;
            AsyncHandler = asyncHandler;
        }

        /// <summary>
        /// Gets the controller type.
        /// </summary>
        public Type ControllerType { get; }

        /// <summary>
        /// Gets the handler method metadata.
        /// </summary>
        public MethodInfo MethodInfo { get; }

        /// <summary>
        /// Gets a function that can be invoked to handle the command.
        /// </summary>
        public Func<object, IEnumerable<string>, CancellationToken, Task> AsyncHandler { get; }

        /// <inheritdoc />
        public override string ToString() => $"{ControllerType.Name}.{MethodInfo.Name}";
    }
}