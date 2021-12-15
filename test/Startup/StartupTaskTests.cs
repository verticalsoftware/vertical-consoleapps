using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Vertical.ConsoleApplications.Services;
using Xunit;

namespace Vertical.ConsoleApplications.Test.Startup;

public class StartupTaskTests
{
    public class SomeStartup : IStartupTask
    {
        private readonly Dictionary<string, bool> _service;

        public SomeStartup(Dictionary<string, bool> service)
        {
            _service = service;
        }

        /// <inheritdoc />
        public Task InitializeAsync(string[] args)
        {
            _service["startup-3"] = true;
            return Task.CompletedTask;
        }
    }
    
    [Fact]
    public async Task StartupTasksInvoked()
    {
        var service = new Dictionary<string, bool>();
        var entryArguments = new[] { "arg-1", "arg-2" };

        var services = new ServiceCollection()
            .AddSingleton(service)
            .AddStartupTasks(entryArguments, startup =>
            {
                startup
                    .AddAction(args =>
                    {
                        args.ShouldBe(entryArguments);
                        service["action-1"] = true;
                        return Task.CompletedTask;
                    })
                    .AddAction((sp, args) =>
                    {
                        args.ShouldBe(entryArguments);
                        sp.GetRequiredService<Dictionary<string, bool>>()["action-2"] = true;
                        return Task.CompletedTask;
                    })
                    .AddTask<SomeStartup>();
            })
            .BuildServiceProvider();

        var runner = services.GetRequiredService<StartupTaskRunner>();

        await runner.RunStartupTasksAsync();

        service["action-1"].ShouldBeTrue();
        service["action-2"].ShouldBeTrue();
        service["startup-3"].ShouldBeTrue();
    }
}