using System;
using Shouldly;
using Vertical.ConsoleApplications.Routing;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Routing
{
    public class CommandAttributeTests
    {
        [Theory]
        [InlineData(default(string))]
        [InlineData(" ")]
        public void ConstructorRejectsNullRoute(string? input)
        {
            Should.Throw<ArgumentException>(() => new CommandAttribute(input!));
        }
    }
}