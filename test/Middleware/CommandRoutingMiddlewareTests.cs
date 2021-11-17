using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Vertical.ConsoleApplications.Middleware;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;
using Vertical.Pipelines;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Middleware;

public class CommandRoutingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsyncCallsRouter()
    {
        var router = Substitute.For<ICommandRouter>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var testInstance = new CommandRoutingMiddleware(serviceProvider, router);
        var context = new RequestContext(Array.Empty<string>(), serviceProvider);
        var next = new PipelineDelegate<RequestContext>((_, _) => Task.CompletedTask);

        router.RouteAsync(serviceProvider, context, CancellationToken.None).Returns(Task.CompletedTask);
        
        await testInstance.InvokeAsync(
            context,
            next,
            CancellationToken.None);

        await router.Received(1).RouteAsync(serviceProvider, context, CancellationToken.None);
    }
}