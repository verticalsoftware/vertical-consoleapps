using System;
using System.Threading;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Routing;

namespace Vertical.ConsoleApps.Example
{
    public class HelpFeature
    {
        [CommandRoute("help")]
        public Task Handle(string[] args, CancellationToken cancellationToken)
        {
            Console.WriteLine("Help invoked!");
            return Task.CompletedTask;
        }
    }
}