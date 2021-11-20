using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shouldly;
using Vertical.ConsoleApplications.IO;
using Xunit;

namespace Vertical.ConsoleApplications.Test;

public class ConsoleHostBuilderTests
{
    [Fact]
    public void CreateDefaultRegistersCoreServices()
    {
        var host = ConsoleHostBuilder.CreateDefault().Build();
        var services = host.Services;

        services.GetRequiredService<IHostedService>().ShouldNotBeNull();
        services.GetRequiredService<IConsoleInputAdapter>().ShouldNotBeNull();
    }
}