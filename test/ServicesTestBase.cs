using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;

namespace Vertical.ConsoleApplications.Test;

public abstract class ServicesTestBase
{
    protected readonly IServiceCollection Services = new ServiceCollection();
    protected readonly IHostBuilder HostBuilder = Substitute.For<IHostBuilder>();

    protected ServicesTestBase()
    {
        Services.AddSingleton(Substitute.For<IHostApplicationLifetime>());
        HostBuilder
            .ConfigureServices(Arg.Any<Action<HostBuilderContext, IServiceCollection>>())
            .Returns(callInfo =>
            {
                var action = callInfo.Arg<Action<HostBuilderContext, IServiceCollection>>();
                action(null!, Services);
                return HostBuilder;
            });
    }

    protected IServiceProvider ServiceProvider => Services.BuildServiceProvider();
}