using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Vertical.ConsoleApplications.Commands;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Commands
{
    public class HandlerDelegateFactoryTests
    {
        public class TestController
        {
            public int Invocations { get; private set; }
            public string[]? Arguments { get; private set; } = default!;
            public CancellationToken CancellationToken { get; private set; }

            public Task Handle(IEnumerable<string> arguments, CancellationToken cancellationToken)
            {
                Arguments = arguments.ToArray();
                CancellationToken = cancellationToken;
                Invocations++;
                return Task.CompletedTask;
            }

            public Task HandleWrongParameters(string someString) => Task.CompletedTask;

            public bool HandleWrongReturnType() => true;
        }

        [Fact]
        public async Task CreateReturnsInvokableDelegateForType()
        {
            using var cancelTokenSource = new CancellationTokenSource();
            var function = HandlerDelegateFactory.Create(typeof(TestController).GetMethod("Handle")!);
            var instance = new TestController();
            await function(instance, new[] {"red", "blue"}, cancelTokenSource.Token);
            instance.Invocations.ShouldBe(1);
            instance.Arguments.ShouldBe(new[]{"red", "blue"});
            instance.CancellationToken.ShouldBe(cancelTokenSource.Token);
        }

        [Fact]
        public void CreateWithNonMatchedParametersThrows()
        {
            Should.Throw<InvalidOperationException>(() => HandlerDelegateFactory.Create(typeof(TestController)
                .GetMethod("HandleWrongParameters")!));
        }
        
        [Fact]
        public void CreateWithNonMatchedReturnTypeThrows()
        {
            Should.Throw<InvalidOperationException>(() => HandlerDelegateFactory.Create(typeof(TestController)
                .GetMethod("HandleWrongReturnType")!));
        }
    }
}