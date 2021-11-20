using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Vertical.ConsoleApplications.Middleware;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;
using Vertical.Pipelines;
using Xunit;

namespace Vertical.ConsoleApplications.Test;

public class PipelineBuilderExtensionsTests : ServicesTestBase
{
    [Fact]
    public void UseExitCommandRegistersMiddleware()
    {
        HostBuilder.Configure(app => app.UseExitCommand("exit"));
        ServiceProvider
            .GetServices<IPipelineMiddleware<RequestContext>>()
            .Count(svc => svc is ExitCommandMiddleware)
            .ShouldBe(1);
    }
    
    [Fact]
    public void UseExitCommandsRegistersMiddleware()
    {
        HostBuilder.Configure(app => app.UseExitCommands(new[]{"quit", "exit"}));
        ServiceProvider
            .GetServices<IPipelineMiddleware<RequestContext>>()
            .Count(svc => svc is ExitCommandMiddleware)
            .ShouldBe(1);
    }

    [Fact]
    public void UseArgumentReplacementRegistersMiddleware()
    {
        HostBuilder.Configure(app => app.UseArgumentReplacement("replace", "new-value"));
        ServiceProvider
            .GetServices<IPipelineMiddleware<RequestContext>>()
            .Count(svc => svc is ArgumentReplacementMiddleware)
            .ShouldBe(1);
    }

    [Fact]
    public void UseArgumentReplacementFuncRegistersMiddleware()
    {
        HostBuilder.Configure(app => app.UseArgumentReplacement(value => "replacement"));
        ServiceProvider
            .GetServices<IPipelineMiddleware<RequestContext>>()
            .Count(svc => svc is ArgumentReplacementMiddleware)
            .ShouldBe(1);
    }
    
    [Fact]
    public void UseEnvironmentVariableTokensRegistersMiddleware()
    {
        HostBuilder.Configure(app => app.UseEnvironmentVariableTokens());
        ServiceProvider
            .GetServices<IPipelineMiddleware<RequestContext>>()
            .Count(svc => svc is ArgumentReplacementMiddleware)
            .ShouldBe(1);
    }
    
    [Fact]
    public void UseSpecialFolderTokensRegistersMiddleware()
    {
        HostBuilder.Configure(app => app.UseSpecialFolderTokens());
        ServiceProvider
            .GetServices<IPipelineMiddleware<RequestContext>>()
            .Count(svc => svc is ArgumentReplacementMiddleware)
            .ShouldBe(1);
    }

    [Fact]
    public void UseRoutingRegistersMiddleware()
    {
        Services.AddSingleton(Substitute.For<ICommandRouter>());
        HostBuilder.Configure(app => app.UseRouting(_ => { }));
        ServiceProvider
            .GetServices<IPipelineMiddleware<RequestContext>>()
            .Count(svc => svc is CommandRoutingMiddleware)
            .ShouldBe(1);
    }
}