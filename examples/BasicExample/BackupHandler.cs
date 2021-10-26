using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;

namespace BasicExample
{
    [CommandHandler]
    public class BackupHandler
    {
        private readonly ILogger<BackupHandler> _logger;

        public BackupHandler(ILogger<BackupHandler> logger)
        {
            _logger = logger;
        }

        [Command("backup")]
        public Task BackupAsync(CommandContext context, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Backup file {src} to {dest}",
                context.Arguments[0],
                context.Arguments[1]);

            return Task.CompletedTask;
        }
        
        [Command("copy")]
        public Task CopyAsync(CommandContext context, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Copy file {src} to {dest}",
                context.Arguments[0],
                context.Arguments[1]);

            return Task.CompletedTask;
        }
    }
}