using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Vertical.ConsoleApplications.IO;
using Vertical.ConsoleApplications.Providers;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Providers;

public class ProviderBuilderTests
{
    private readonly ProviderBuilder _testInstance = new ProviderBuilder(new ServiceCollection());
    
    [Fact]
    public async Task AddArgumentsRegistersProvider()
    {
        _testInstance.AddArguments(new[] { "test" });

        await VerifyArgsAsync<StaticArgumentsProvider>("test");
    }

    [Fact]
    public async Task AddArgumentStringRegistersProvider()
    {
        _testInstance.AddArgumentString("test parameter");

        await VerifyArgsAsync<StaticArgumentsProvider>("test", "parameter");
    }

    [Fact]
    public async Task AddEntryArgumentsRegistersProvider()
    {
        _testInstance.AddEntryArguments(new[] { "test" });

        await VerifyArgsAsync<StaticArgumentsProvider>("test");
    }

    [Fact]
    public async Task AddEnvironmentVariablesRegistersProvider()
    {
        var key = "_TEST" + Guid.NewGuid().ToString("N");
        Environment.SetEnvironmentVariable(key, "test parameter", EnvironmentVariableTarget.Process);
        _testInstance.AddEnvironmentVariable(key);

        await VerifyArgsAsync<StaticArgumentsProvider>("test", "parameter");
    }

    private class TestProvider : IArgumentsProvider
    {
        /// <inheritdoc />
        public Task InvokeArgumentsAsync(Func<string[], Task> handler, CancellationToken cancellationToken)
        {
            return handler(new[] { "test" });
        }
    }

    [Fact]
    public async Task AddProviderRegistersProvider()
    {
        _testInstance.AddProvider<TestProvider>();
        await VerifyArgsAsync<TestProvider>("test");
    }

    [Fact]
    public async Task AddScriptRegistersProvider()
    {
        _testInstance.AddScript("Resources/test_args.txt");
        await VerifyArgsAsync<ScriptFileArgumentsProvider>("test", "parameter");
    }

    [Fact]
    public async Task AddScriptsRegistersProvider()
    {
        _testInstance.AddScripts("Resources", "*.txt");
        await VerifyArgsAsync<ScriptFileArgumentsProvider>("test", "parameter");
    }

    [Fact]
    public async Task AddInteractiveConsoleRegistersProvider()
    {
        var console = Substitute.For<IConsoleInputAdapter>();
        console.ReadLine().Returns("test parameter");
        _testInstance
            .AddInteractiveConsole()
            .ApplicationServices.AddSingleton(console);

        await VerifyArgsAsync<InteractiveArgumentsProvider>("test", "parameter");
    }

    private async Task VerifyArgsAsync<T>(params string[] expectedArgs)
    {
        using var cancelTokenSource = new CancellationTokenSource();
        
        var providers = _testInstance
            .ApplicationServices
            .BuildServiceProvider()
            .GetServices<IArgumentsProvider>();

        var provider = providers.Single();
        
        provider.ShouldBeOfType<T>();

        var argsList = new List<string>();

        await provider.InvokeArgumentsAsync(args =>
        {
            cancelTokenSource.Cancel();
            argsList.AddRange(args);
            return Task.CompletedTask;
        }, cancelTokenSource.Token);
        
        argsList.ShouldBe(expectedArgs);
    }
}