using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using Vertical.ConsoleApplications.Middleware;
using Vertical.ConsoleApplications.Pipeline;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Middleware
{
    public class ArgumentReplacementMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsyncReplacesPerFunction()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            var args = new[] { "red", "green", "blue" };
            var testInstance = new ArgumentReplacementMiddleware(color =>
                color == "green" ? "pink" : color);

            await testInstance.InvokeAsync(
                new RequestContext(args, new RequestItems(), Substitute.For<IHostApplicationLifetime>(), serviceProvider),
                (_, _) => Task.CompletedTask,
                CancellationToken.None);
            
            args[1].ShouldBe("pink");
        }
    }
}