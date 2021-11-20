using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shouldly;
using Vertical.ConsoleApplications.Pipeline;
using Xunit;

namespace Vertical.ConsoleApplications.Test;

public class ConsoleHostedServiceTests
{
    public class RequestInitializer : IRequestInitializer
    {
        /// <inheritdoc />
        public void Initialize(RequestContext context)
        {
            context.Items.Set(this);
        }
    }

    [Fact]
    public async Task InvokeCommandProviderCancelsWithToken()
    {
        var tested = false;
        
        var host = ConsoleHostBuilder
            .CreateDefault()
            .ConfigureServices(services => services.AddSingleton<IRequestInitializer, RequestInitializer>())
            .ConfigureProviders(providers => providers.AddArguments(new[] { "args" }))
            .Configure(app =>
            {
                app.Use((context, next, cancel) =>
                {
                    context.Arguments.Single().ShouldBe("args");
                    context.Items.Get<RequestInitializer>().ShouldNotBeNull();
                    tested = true;
                    return next(context, cancel);
                });
            });

        await host.RunConsoleAsync(CancellationToken.None);

        tested.ShouldBeTrue();
    }
}