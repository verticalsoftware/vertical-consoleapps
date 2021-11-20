using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Vertical.ConsoleApplications.Middleware;
using Vertical.ConsoleApplications.Pipeline;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Middleware;

public class ExitCommandMiddlewareTests
{
    [Theory]
    [InlineData("quit", 0)]
    [InlineData("exit", 1)]
    public async Task InvokeAsyncCallsStopApplicationWhenExpected(string arg, int calls)
    {
        var appLifetime = Substitute.For<IHostApplicationLifetime>();
        var testInstance = new ExitCommandMiddleware(
            new[] { "exit" },
            appLifetime,
            NullLogger<ExitCommandMiddleware>.Instance);

        await testInstance.InvokeAsync(new RequestContext(new[] { arg },
                new RequestItems(),
                Substitute.For<IHostApplicationLifetime>(),
                Substitute.For<IServiceProvider>()),
            (_, _) => Task.CompletedTask,
            CancellationToken.None);
        
        appLifetime.Received(calls).StopApplication();
    }
}