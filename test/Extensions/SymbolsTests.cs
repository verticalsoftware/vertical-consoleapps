using Shouldly;
using Vertical.ConsoleApplications.Extensions;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Extensions
{
    public class SymbolsTests
    {
        [Fact]
        public void ReplaceSymbolsSendsCorrectSymbol()
        {
            var symbol = string.Empty;

            "$TEST".ReplaceSymbols(sym => symbol = sym);
            
            symbol.ShouldBe("TEST");
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData("", null, "")]
        [InlineData("none", null, "none")]
        [InlineData("$PATH", null, "$PATH")]
        [InlineData("$PATH", "", "")]
        [InlineData("$PATH", "the-path", "the-path")]
        [InlineData("The path is $PATH", "the-path", "The path is the-path")]
        public void ReplaceSymbolsReplacesValue(string? input, string? value, string? expected)
        {
            input.ReplaceSymbols(_ => value).ShouldBe(expected);
        }
    }
}