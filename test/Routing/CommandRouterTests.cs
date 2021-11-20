using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;
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
}