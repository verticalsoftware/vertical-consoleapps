using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;

namespace FeatureSliceRouting.Features
{
    [Command("cat")]
    public class CatFile : ICommandHandler
    {
        /// <inheritdoc />
        public async Task HandleAsync(RequestContext context, CancellationToken cancellationToken)
        {
            var args = context.Arguments;

            foreach (var arg in args)
            {
                try
                {
                    Console.WriteLine($"Path = {arg}");
                    Console.WriteLine(new string('-', Console.WindowWidth));
                    
                    foreach (var line in await File.ReadAllLinesAsync(arg, cancellationToken))
                    {
                        Console.WriteLine(line);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"[{exception.GetType()}]: {exception.Message}");
                }
            }
        }
    }
}