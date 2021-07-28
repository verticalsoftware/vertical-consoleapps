using System;
using Shouldly;
using Vertical.ConsoleApplications.Extensions;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Extensions
{
    public class ArgumentsTests
    {
        [Theory]
        [InlineData("", null)]
        [InlineData("test", "test")]
        [InlineData("arg1 arg2", "arg1|arg2")]
        [InlineData("quoted \"test\"", "quoted|test")]
        [InlineData("quoted \"with spaces\"", "quoted|with spaces")]
        [InlineData("escaping \"\"quotes\"\"", "escaping|\"quotes\"")]
        public void ToArgumentsArrayReturnsExpected(string input, string? expected)
        {
            input.GetEscapedArguments().ShouldBe(expected?.Split("|") ?? Array.Empty<string>());
        }
    }
}