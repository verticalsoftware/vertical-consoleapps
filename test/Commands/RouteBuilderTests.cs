using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Vertical.ConsoleApplications.Commands;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Commands
{
    public class RouteBuilderTests
    {
        public class ControllerA
        {
            public Task Handle(IEnumerable<string> args, CancellationToken cancellationToken) => Task.CompletedTask;
        }

        public class ControllerB
        {
            [Command("handler")]
            public Task Handle(IEnumerable<string> args, CancellationToken cancellationToken) => Task.CompletedTask;
        }

        [Command("controller")]
        public class ControllerC
        {
            [Command("handler")]
            public Task Handle(IEnumerable<string> arg, CancellationToken cancellationToken) => Task.CompletedTask;
        }

        public class ControllerD
        {
            [Command("handler1")]
            public Task Handle(IEnumerable<string> arg, CancellationToken cancellationToken) => Task.CompletedTask;
            [Command("handler2")]
            public Task Handle2(IEnumerable<string> arg, CancellationToken cancellationToken) => Task.CompletedTask;
        }

        [Fact]
        public void BuildRouteMapsReturnsEmptyForNonMatchedMetadata()
        {
            RouteBuilder.BuildRouteMaps(typeof(ControllerA)).ShouldBeEmpty();
        }

        [Fact]
        public void BuildRouteMapsReturnsHandlerPath()
        {
            var kv = RouteBuilder.BuildRouteMaps(typeof(ControllerB)).Single();
            kv.Key.ShouldBe("handler");
            kv.Value.ShouldNotBeNull();
        }
        
        [Fact]
        public void BuildRouteMapsReturnsBaseAndHandlerPath()
        {
            var kv = RouteBuilder.BuildRouteMaps(typeof(ControllerC)).Single();
            kv.Key.ShouldBe("controller handler");
            kv.Value.ShouldNotBeNull();
        }

        [Fact]
        public void BuildMultiRouteMapReturnsAllRoutes()
        {
            var items = RouteBuilder.BuildRouteMaps(typeof(ControllerD)).ToArray();
            items.Length.ShouldBe(2);
            items.Count(kv => kv.Key == "handler1").ShouldBe(1);
            items.Count(kv => kv.Key == "handler2").ShouldBe(1);
            items.All(kv => kv.Value != null).ShouldBeTrue();
        }
    }
}