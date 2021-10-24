using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;

namespace BasicExample
{
    public class MyController
    {
        private readonly ILogger<MyController> _logger;

        public MyController(ILogger<MyController> logger)
        {
            _logger = logger;
        }
        
        [Command("route")]
        public Task Handle(CommandContext context, CancellationToken cancel)
        {
            _logger.LogInformation("You routed stuff!");
            return Task.CompletedTask;
        }
    }
}