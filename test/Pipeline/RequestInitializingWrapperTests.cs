using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using Vertical.ConsoleApplications.Pipeline;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Pipeline;

public class RequestInitializingWrapperTests
{
    [Fact]
    public void InitializesInvokesAction()
    {
        var services = new ServiceCollection().AddSingleton(new RequestItems())
            .BuildServiceProvider();
        var context = new RequestContext(Array.Empty<string>(), new RequestItems(), Substitute.For<IHostApplicationLifetime>(), services);
        var testInstance = new RequestInitializingWrapper(ctx => ctx.Items.Set("test"));
        
        testInstance.Initialize(context);
        
        context.Items.Get<string>().ShouldBe("test");
    }
}