using System;
using Shouldly;
using Vertical.ConsoleApplications.Utilities;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Utilities
{
    public class ArgumentHelpersTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData(" ", "")]
        [InlineData("  ", "")]
        [InlineData("test","test")]
        [InlineData(" test","test")]
        [InlineData("test ","test")]
        [InlineData("arg1 arg2", "arg1+arg2")]
        [InlineData("arg1   arg2", "arg1+arg2")]
        [InlineData(" arg1 arg2 ", "arg1+arg2")]
        [InlineData("\"arg1 arg2\"", "arg1 arg2")]
        [InlineData("\" arg1 arg2 \"", "arg1 arg2")]
        [InlineData("test even more lines", "test+even+more+lines")]
        public void SplitFromStringReturnsExpected(string input, string expected)
        {
            var expectedArray = string.IsNullOrWhiteSpace(expected)
                ? Array.Empty<string>()
                : expected.Split('+');
            
            ArgumentHelpers.SplitFromString(input).ShouldBe(expectedArray);
        }
    }
}