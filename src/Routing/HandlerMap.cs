using System;
using System.Collections.Generic;
using System.Linq;

namespace Vertical.ConsoleApplications.Routing
{
    public class HandlerMap : Dictionary<string, Func<IServiceProvider, ICommandHandler>>
    {
        internal HandlerMap(IEnumerable<RouteDescriptor> routeDescriptors) : base(routeDescriptors
            .ToDictionary(dsc => dsc.Route, dsc => dsc.ImplementationFactory))
        {
        }
    }
}