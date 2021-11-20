using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Providers;
using Vertical.Pipelines;
using Xunit;

namespace Vertical.ConsoleApplications.Test;

public class HostBuilderExtensionsTests : ServicesTestBase
{
    [Fact]
    public void ConfigureProvidersRoutesToProviderBuilder()
    {
        HostBuilder.ConfigureProviders(provider => provider.AddArguments(new[] { "test" }));
        ServiceProvider
            .GetServices<IArgumentsProvider>()
            .Single()
            .ShouldNotBeNull();
    }

    [Fact]
    public void ConfigureRoutesToPipelineBuilder()
    {
        HostBuilder.Configure(app => app.Use((context, next, ct) => next(context, ct)));
        ServiceProvider
            .GetServices<IPipelineMiddleware<RequestContext>>()
            .Single()
            .ShouldNotBeNull();
    }
}