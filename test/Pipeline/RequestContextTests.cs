using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using Vertical.ConsoleApplications.Pipeline;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Pipeline;

public class RequestContextTests
{
    [Fact]
    public void ApplicationLifetimeMaterializesLazy()
    {
        var appLifetime = Substitute.For<IHostApplicationLifetime>();
        var services = new ServiceCollection().AddSingleton(appLifetime).BuildServiceProvider();
        
        new RequestContext(Array.Empty<string>(), services)
            .ApplicationLifetime
            .ShouldBe(appLifetime);
    }
    
    [Fact]
    public void ItemsMaterializesLazy()
    {
        var items = new RequestItems();
        var services = new ServiceCollection().AddSingleton(items).BuildServiceProvider();
        
        new RequestContext(Array.Empty<string>(), services)
            .Items
            .ShouldBe(items);
    }
}