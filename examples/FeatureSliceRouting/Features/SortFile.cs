using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;

namespace FeatureSliceRouting.Features
{
    [Command("sort")]
    public class SortFile : ICommandHandler
    {
        /// <inheritdoc />
        public async Task HandleAsync(RequestContext context, CancellationToken cancellationToken)
        {
            var args = context.Arguments;
            var sourceFile = args.Length > 0 ? args[0] : null;
            var sortOrder = args.Length > 1 ? args[1] : null;

            if (sourceFile == null)
            {
                throw new ApplicationException("Expected a source file parameter.");
            }

            try
            {
                var content = await File.ReadAllLinesAsync(sourceFile, cancellationToken);

                foreach (var line in GetSortedContent(content, sortOrder))
                {
                    Console.WriteLine(line);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"[{exception.GetType()}]: {exception.Message}");
            }
        }

        private static IEnumerable<string> GetSortedContent(string[] content, string? sortOrder)
        {
            return sortOrder?.ToLower() switch
            {
                "asc" => content.OrderBy(str => str),
                "desc" => content.OrderByDescending(str => str),
                _ => content
            };
        }
    }
}