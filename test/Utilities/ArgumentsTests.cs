using Shouldly;
using Vertical.ConsoleApplications.Utilities;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Utilities
{
    public class ArgumentsTests
    {
        [Theory]
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
            Arguments.SplitFromString(input).ShouldBe(expected.Split('+'));
        }
    }
}