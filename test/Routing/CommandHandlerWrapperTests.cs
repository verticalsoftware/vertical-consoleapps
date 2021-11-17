using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Routing;

public class CommandHandlerWrapperTests
{
    [Fact]
    public async Task HandleAsyncInvokesFunc()
    {
        var context = new RequestContext(Array.Empty<string>(), Substitute.For<IServiceProvider>());
        var cancelToken = CancellationToken.None;
        var invoked = false;
        var testInstance = new CommandHandlerWrapper((ctx, ct) =>
        {
            invoked = true;
            ctx.ShouldBe(context);
            ct.ShouldBe(cancelToken);
            return Task.CompletedTask;
        });

        await testInstance.HandleAsync(context, cancelToken);
    }
}