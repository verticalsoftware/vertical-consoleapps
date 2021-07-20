using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Vertical.ConsoleApplications.Parsing;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Parsing
{
    public class CommandArgumentsTests
    {
        [Theory]
        [InlineData("copy", "-o", false)]
        [InlineData("copy -o", "-o", true)]
        [InlineData("copy --overwrite", "-o|--overwrite", true)]
        public void GetSwitchReturnsExpected(string input, string template, bool expected)
        {
            input.SplitToArguments().GetSwitch(template).ShouldBe(expected);
        }

        [Theory]
        [InlineData("copy", "--src", null)]
        [InlineData("copy --src path", "--src", "path")]
        [InlineData("copy --dest dest --src src", "--src", "src")]
        [InlineData("copy --dest dest --src src --batch 10mb", "--src", "src")]
        public void TryGetOptionReturnsExpected(string input, string template, string expected)
        {
            var result = input.SplitToArguments().TryGetOption(template, out var value);
            result.ShouldBe(expected != null);
            value.ShouldBe(expected);
        }

        [Theory]
        [InlineData("copy", "--color", null)]
        [InlineData("copy --color red", "--color", "red")]
        [InlineData("copy --color red --color green --color blue", "--color", "red+green+blue")]
        public void GetOptionsReturnsExpected(string input, string template, string? expected)
        {
            var result = input.SplitToArguments().GetOptions(template).ToArray();
            result.ShouldBe(expected?.Split('+') ?? Array.Empty<string>());
        }
        
        [Theory, MemberData(nameof(SplitToArgumentsTheories))]
        public void SplitToArgumentsReturnsExpected(string input, string[] expected)
        {
            var result = input.SplitToArguments().ToArray();
            
            result.ShouldBe(expected);
        }

        public static IEnumerable<object[]> SplitToArgumentsTheories => new[]
        {
            new object[] {null, Array.Empty<string>()},
            new object[] {"", Array.Empty<string>()},
            new object[] {"foo", new[]{"foo"}},
            new object[] {"foo bar", new[]{"foo", "bar"}},
            new object[] {"foo \"foo bar\"", new[]{"foo", "foo bar"}},
            new object[] {"foo \"foo \"\"bar\"\"\"", new[]{"foo", "foo \"bar\""}}
        };
    }
}