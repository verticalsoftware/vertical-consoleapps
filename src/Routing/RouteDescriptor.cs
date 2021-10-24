using System;

namespace Vertical.ConsoleApplications.Routing
{
    /// <summary>
    /// Represents the registration of a route with a command handler implementation.
    /// </summary>
    /// <param name="Route">The route handled by the command.</param>
    /// <param name="ImplementationFactory">A factory method that creates the instance.</param>
    public record RouteDescriptor(string Route, Func<IServiceProvider, ICommandHandler> ImplementationFactory);
}