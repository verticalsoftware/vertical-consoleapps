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

public class DecoratedMethodHandlerTests
{
    public class TestController
    {
        public Task InvokeAsync(RequestContext context, CancellationToken cancellationToken)
        {
            context.Items.Set("invoked");
            return Task.CompletedTask;
        }    
    }
    
    [Fact]
    public async Task HandleAsyncInvokesHandler()
    {
        var testInstance = new DecoratedMethodHandler<TestController>(new TestController(),
            (controller, context, ct) => controller.InvokeAsync(context, ct));

        var items = new RequestItems();
        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider.GetService(typeof(RequestItems)).Returns(items);
        var requestContext = new RequestContext(Array.Empty<string>(), 
            items,
            Substitute.For<IHostApplicationLifetime>(),
            serviceProvider);

        await testInstance.HandleAsync(requestContext, CancellationToken.None);
        
        items.Get<string>().ShouldBe("invoked");
    }
}