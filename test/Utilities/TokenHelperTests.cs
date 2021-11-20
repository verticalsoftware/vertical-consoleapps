using System;
using Shouldly;
using Vertical.ConsoleApplications.Utilities;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Utilities
{
    public class TokenHelperTests
    {
        [Theory]
        [InlineData("arg1", "arg1")]
        [InlineData("arg1+arg2", "arg1")]
        [InlineData("arg1+arg2+arg3", "arg1")]
        [InlineData("arg1+arg2+arg3", "arg1 arg2")]
        [InlineData("arg1+arg2+arg3", "arg1 arg2 arg3")]
        public void IsCommandMatchPositives(string args, string command)
        {
            var split = args.Split('+');

            TokenHelpers.IsCommandMatch(split, command).ShouldBeTrue();
        }

        [Fact]
        public void ReplaceEnvironmentVariablesReplacesSymbols()
        {
            var result = TokenHelpers.ReplaceEnvironmentVariables("Path is $PATH");
            result.ShouldBe($"Path is {Environment.GetEnvironmentVariable("PATH")}");
        }

        [Fact]
        public void ReplaceSpecialFolderTokensReplacesSymbols()
        {
            var result = TokenHelpers.ReplaceSpecialFolderPaths("Path is $ApplicationData");
            result.ShouldBe($"Path is {Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}");
        }
    }
}