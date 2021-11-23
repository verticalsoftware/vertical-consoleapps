using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;
using Vertical.ConsoleApplications.Utilities;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Routing;

public class CommandRouterTests
{
    private readonly RouteDescriptor[] _descriptors =
    {
        new ("command", _ => new CommandHandlerWrapper((context, _) =>
        {
            context.Items.Set("command");
            return Task.CompletedTask;
        })),
        new ("command subcommand1", _ => new CommandHandlerWrapper((context, _) =>
        {
            context.Items.Set("command subcommand1");
            return Task.CompletedTask;
        })),
        new ("command subcommand2", _ => new CommandHandlerWrapper((context, _) =>
        {
            context.Items.Set("command subcommand2");
            return Task.CompletedTask;
        })),
        new ("command subcommand sub-subcommand", _ => new CommandHandlerWrapper((context, _) =>
        {
            context.Items.Set("command subcommand sub-subcommand");
            return Task.CompletedTask;
        })),
        new ("command sub", _ => new CommandHandlerWrapper((context, _) =>
        {
            context.Items.Set("command sub");
            return Task.CompletedTask;
        }))
    };
    
    [Theory]
    [InlineData("command")]
    [InlineData("command subcommand1")]
    [InlineData("command subcommand2")]
    [InlineData("command subcommand sub-subcommand")]
    [InlineData("command sub")]
    public async Task RouteAsyncSelectsDescriptor(string command)
    {
        var testInstance = new CommandRouter(_descriptors);
        var serviceProvider = Substitute.For<IServiceProvider>();
        var context = new RequestContext(command.Split(' '), 
            new RequestItems(), 
            Substitute.For<IHostApplicationLifetime>(),
            serviceProvider);
        
        await testInstance.RouteAsync(context, CancellationToken.None);

        context.Items.Get<string>().ShouldBe(command);
    }

    [Theory]
    [InlineData("dotnet", "dotnet", "")]
    [InlineData("dotnet add package", "dotnet add package", "")]
    [InlineData("dotnet add package", "dotnet add package vertical-commandline", "vertical-commandline")]
    [InlineData("dotnet add package", "dotnet add package vertical-commandline -v 3.0.0", "vertical-commandline+-v+3.0.0")]
    public async Task RouteAsyncParametersSelectsDescriptor(string route, string command, string expectedArgs)
    {
        var args = new List<string>();
        var descriptor = new RouteDescriptor(route, _ => new CommandHandlerWrapper((context, _) =>
        {
            args.AddRange(context.Arguments);
            return Task.CompletedTask;
        }));
        var router = new CommandRouter(new[] { descriptor });
        var context = new RequestContext(ArgumentHelpers.SplitFromString(command),
            new RequestItems(),
            Substitute.For<IHostApplicationLifetime>(),
            Substitute.For<IServiceProvider>());

        await router.RouteAsync(context, CancellationToken.None);
        
        args.ShouldBe(expectedArgs.Split('+').Where(str => !string.IsNullOrWhiteSpace(str)));
    }
}