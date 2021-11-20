using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;
using Xunit;

namespace Vertical.ConsoleApplications.Test;

public class ServiceCollectionExtensionsTests : ServicesTestBase
{
    [Fact]
    public void AddCommandRoutingRegistersServices()
    {
        Services.AddCommandRouting();
        ServiceProvider.GetService<ICommandRouter>().ShouldNotBeNull();
    }

    private class RequestInitializer : IRequestInitializer
    {
        public void Initialize(RequestContext context)
        {
        }
    }

    [Fact]
    public void AddRequestInitializerRegistersServices()
    {
        Services.AddRequestInitializer<RequestInitializer>();
        ServiceProvider
            .GetServices<IRequestInitializer>()
            .Count(svc => svc is RequestInitializer)
            .ShouldBe(1);
    }
    
    [Fact]
    public void AddRequestInitializerWithFactoryRegistersServices()
    {
        Services.AddRequestInitializer(_ => new RequestInitializer());
        ServiceProvider
            .GetServices<IRequestInitializer>()
            .Count(svc => svc is RequestInitializer)
            .ShouldBe(1);
    }

    [Fact]
    public void AddRequestInitializerFunctionRegistersServices()
    {
        Services.AddRequestInitializer(_ => { });
        ServiceProvider
            .GetServices<IRequestInitializer>()
            .Count(svc => svc is RequestInitializingWrapper)
            .ShouldBe(1);
    }
}