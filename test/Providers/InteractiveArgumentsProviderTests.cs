using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Vertical.ConsoleApplications.IO;
using Vertical.ConsoleApplications.Providers;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Providers;

public class InteractiveArgumentsProviderTests
{
    [Fact]
    public async Task InvokeArgumentsAsyncCallsHandlers()
    {
        var inputAdapter = Substitute.For<IConsoleInputAdapter>();
        inputAdapter.ReadLine().Returns("test parameter");

        using var cancelTokenSource = new CancellationTokenSource();
        var argsList = new List<string>();
        var promptInvoked = false;
        var prompt = new Action(() =>
        {
            promptInvoked = true;
        });

        var testInstance = new InteractiveArgumentsProvider(
            inputAdapter,
            prompt);

        await testInstance.InvokeArgumentsAsync(
            args =>
            {
                argsList.AddRange(args);
                cancelTokenSource.Cancel();
                return Task.CompletedTask;
            },
            cancelTokenSource.Token);
        
        argsList[0].ShouldBe("test");
        argsList[1].ShouldBe("parameter");
        promptInvoked.ShouldBeTrue();
    }
}