using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.CommandLine.Configuration;
using Vertical.ConsoleApplications.Routing;

namespace BasicExample
{
    [CommandHandler("backup")]
    public class BackupArgumentHandler : CommandArgumentHandler<BackupArgumentHandler.MyOptions>
    {
        private readonly ILogger<BackupArgumentHandler> _logger;

        public class MyOptions
        {
            public string Source { get; set; }
            public string Destination { get; set; }
        }

        /// <inheritdoc />
        public BackupArgumentHandler(ILogger<BackupArgumentHandler> logger) : base(ConfigureOptions)
        {
            _logger = logger;
        }

        private static void ConfigureOptions(ApplicationConfiguration<MyOptions> config)
        {
            config
                .Help.UseContent(new[] { "Performs a backup of one file to another." })
                .PositionArgument(arg => arg.Map.ToProperty(opt => opt.Source))
                .PositionArgument(arg => arg.Map.ToProperty(opt => opt.Destination));
        }

        /// <inheritdoc />
        protected override Task ExecuteAsync(MyOptions options, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Copy file {source} to {dest}", options.Source, options.Destination);
            return Task.CompletedTask;
        }
    }
}