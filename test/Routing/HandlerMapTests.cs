using NSubstitute;
using Shouldly;
using Vertical.ConsoleApplications.Routing;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Routing;

public class HandlerMapTests
{
    [Fact]
    public void ConstructBuildsDictionary()
    {
        RouteDescriptor[] descriptors =
        {
            new ("red", sp => Substitute.For<ICommandHandler>()),
            new ("green", sp => Substitute.For<ICommandHandler>()),
            new ("blue", sp => Substitute.For<ICommandHandler>())
        };

        var handlerMap = new HandlerMap(descriptors);
        
        handlerMap.ContainsKey("red").ShouldBeTrue();
        handlerMap.ContainsKey("green").ShouldBeTrue();
        handlerMap.ContainsKey("blue").ShouldBeTrue();
    }
}