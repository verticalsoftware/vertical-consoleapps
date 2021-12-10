using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using Vertical.ConsoleApplications.Middleware;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;
using Vertical.Pipelines;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Middleware;

public class UnmatchedRouteMiddlewareTests
{
    [Fact]
    public async Task HandleAsyncRoutesToActionWhenRouteNotMatched()
    {
        var context = new RequestContext(new[] { "command" },
            new RequestItems(),
            Substitute.For<IHostApplicationLifetime>(),
            Substitute.For<IServiceProvider>());

        RequestContext? invokedContext = default;
        
        var next = new PipelineDelegate<RequestContext>((_, _) => Task.CompletedTask);
        var testInstance = new UnmatchedRouteMiddleware(ctx => invokedContext = ctx );

        await testInstance.InvokeAsync(context, next, CancellationToken.None);
        
        invokedContext.ShouldBe(context);
    }

    [Fact]
    public async Task HandleAsyncDoesNotRouteWhenMatched()
    {
        var invoked = false;
        
        var context = new RequestContext(new[] { "command" },
            new RequestItems(),
            Substitute.For<IHostApplicationLifetime>(),
            Substitute.For<IServiceProvider>());

        var next = new PipelineDelegate<RequestContext>((ctx, _) =>
        {
            ctx.Items.Set(new RouteDescriptor("command", _ => Substitute.For<ICommandHandler>()));
            return Task.CompletedTask;
        });

        var testInstance = new UnmatchedRouteMiddleware(_ => invoked = true);

        await testInstance.InvokeAsync(context, next, CancellationToken.None);
        
        invoked.ShouldBeFalse();
    }
}