using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.Utilities;

namespace Vertical.ConsoleApplications.Providers
{
    /// <summary>
    /// Reads argument values from script files.
    /// </summary>
    internal class ScriptFileArgumentsProvider : IArgumentsProvider
    {
        private readonly Func<Task<IEnumerable<string>>> _pathFactory;
        private readonly ILogger<ScriptFileArgumentsProvider>? _logger;

        internal ScriptFileArgumentsProvider(
            Func<Task<IEnumerable<string>>> pathFactory,
            ILogger<ScriptFileArgumentsProvider>? logger = null)
        {
            _pathFactory = pathFactory;
            _logger = logger;
        }
        
        /// <inheritdoc />
        public async Task InvokeArgumentsAsync(
            Func<string[], Task> handler, 
            CancellationToken cancellationToken)
        {
            foreach (var path in await _pathFactory())
            {
                var fullPath = Path.GetFullPath(path);
                
                _logger?.LogTrace("Reading arguments from {path}", fullPath);

                await InvokeArgumentsForPathAsync(path, handler, cancellationToken);
            }
        }

        private async Task InvokeArgumentsForPathAsync(
            string path, 
            Func<string[], Task> handler, 
            CancellationToken cancellationToken)
        {
            using var scope = _logger?.BeginScope(Path.GetFileName(path));
            
            var content = await File.ReadAllLinesAsync(path, cancellationToken);

            foreach (var line in content)
            {
                var args = ArgumentHelpers.SplitFromString(line);

                if (args.Length == 0)
                    continue;
                
                // ReSharper disable once CoVariantArrayConversion
                _logger?.LogTrace("Invoke static arguments: {values}", args);

                await handler(args);
            }
        }
    }
}