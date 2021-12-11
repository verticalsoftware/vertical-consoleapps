using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using Vertical.ConsoleApplications.Middleware;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;
using Vertical.Pipelines;
using Vertical.Pipelines.DependencyInjection;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Routing;

public class RoutingBuilderTests
{
    public class TestHandler : ICommandHandler
    {
        public Task HandleAsync(RequestContext context, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    public class TestController
    {
        [Command("route-a")]
        public Task HandleRouteA(RequestContext context, CancellationToken cancellationToken) =>
            Task.CompletedTask;
        
        [Command("route-b")]
        public Task HandleRouteB(RequestContext context, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }

    private readonly RoutingBuilder _testInstance = new RoutingBuilder(new PipelineBuilder<RequestContext>(
        new ServiceCollection(), ServiceLifetime.Singleton));

    [Fact]
    public void MapPerformsRegistration()
    {
        _testInstance.
            Map("route", (_, _) => Task.CompletedTask)
            .ApplicationServices
            .BuildServiceProvider()
            .GetServices<RouteDescriptor>()
            .Single(d => d.Route == "route")
            .ShouldNotBeNull();
    }

    [Fact]
    public void MapHandlerWithProviderPerformsRegistration()
    {
        _testInstance
            .MapHandler("route", sp => Substitute.For<ICommandHandler>())
            .ApplicationServices
            .BuildServiceProvider()
            .GetServices<RouteDescriptor>()
            .Single(d => d.Route == "route")
            .ShouldNotBeNull();
    }

    [Fact]
    public void MapHandlerWithGenericTypePerformsRegistration()
    {
        _testInstance
            .MapHandler<TestHandler>("route")
            .ApplicationServices
            .BuildServiceProvider()
            .GetServices<RouteDescriptor>()
            .Single(d => d.Route == "route")
            .ShouldNotBeNull();
    }

    [Fact]
    public void MapHandlerWithRuntimeTypePerformsRegistration()
    {
        _testInstance
            .MapHandler("route", typeof(TestHandler))
            .ApplicationServices
            .BuildServiceProvider()
            .GetServices<RouteDescriptor>()
            .Single(d => d.Route == "route")
            .ShouldNotBeNull();
    }

    [Fact]
    public void MapControllerWithGenericTypePerformsRegistration()
    {
        var handlers = _testInstance
            .MapHandler<TestController>()
            .ApplicationServices
            .BuildServiceProvider()
            .GetServices<RouteDescriptor>()
            .ToArray();

        handlers.Length.ShouldBe(2);
        handlers.First().Route.ShouldBe("route-a");
        handlers.Last().Route.ShouldBe("route-b");
    }

    [Fact]
    public void MapUnmatchedAddsMiddleware()
    {
        var services = _testInstance
            .MapUnmatchedRoutes(_ => throw new InvalidOperationException())
            .ApplicationServices
            .BuildServiceProvider();

        var middlewares = services.GetServices<IPipelineMiddleware<RequestContext>>();
        
        middlewares.Count(m => m is UnmatchedRouteMiddleware).ShouldBe(1);
    }

    private static RequestContext CreateContext()
    {
        var context = new RequestContext(Array.Empty<string>(), 
            new RequestItems(),
            Substitute.For<IHostApplicationLifetime>(),
            Substitute.For<IServiceProvider>());

        context.Services.GetService(typeof(RequestItems)).Returns(new RequestItems());
        context.Services.GetService(typeof(IHostApplicationLifetime)).Returns(Substitute.For<IHostApplicationLifetime>());
        
        return context;
    }
}