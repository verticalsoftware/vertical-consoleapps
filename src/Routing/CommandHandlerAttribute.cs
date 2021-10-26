using System;

namespace Vertical.ConsoleApplications.Routing
{
    /// <summary>
    /// Implies that a class has command handler methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandHandlerAttribute : Attribute
    {
    }
}