using System;
using System.Reflection;

namespace Vertical.ConsoleApplications.Routing
{
    internal sealed class HandlerDescriptor
    {
        internal HandlerDescriptor(Type implementationType, string command)
        {
            ImplementationType = implementationType;
            Command = command;
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Gets the implementation type.
        /// </summary>
        public Type ImplementationType { get; }

        /// <inheritdoc />
        public override string ToString() => $"'{{Command}}' -> {ImplementationType}";
        
        /// <summary>
        /// Creates a descriptor using a type that presumably has the <see cref="CommandHandlerAttribute"/>
        /// applied.
        /// </summary>
        /// <param name="implementationType">Implementation type.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"><see cref="ImplementationType"/> does not define
        /// <see cref="CommandHandlerAttribute"/></exception>
        internal static HandlerDescriptor FromType(Type implementationType)
        {
            var command = implementationType.GetCustomAttribute<CommandHandlerAttribute>()?.Command
                          ??
                          throw new InvalidOperationException($"Handler class type '{implementationType}' is missing the "
                                                              + $"{typeof(CommandHandlerAttribute)} declaration.");
            
            return new HandlerDescriptor(implementationType, command);
        }
    }
}