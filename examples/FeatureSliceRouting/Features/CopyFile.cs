using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;

namespace FeatureSliceRouting.Features
{
    [Command("copy")]
    public class CopyFile : ICommandHandler
    {
        /// <inheritdoc />
        public Task HandleAsync(RequestContext context, CancellationToken cancellationToken)
        {
            var args = context.Arguments;
            var sourceFile = args.Length > 0 ? args[0] : null;
            var destFile = args.Length > 1 ? args[1] : null;

            if (sourceFile == null || destFile == null)
            {
                throw new ApplicationException("Backup command requires source and destination parameters");
            }

            File.Copy(sourceFile, destFile);

            return Task.CompletedTask;
        }
    }
}