using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Extensions.Logging;
using Vertical.ConsoleApplications.IO;
using Vertical.ConsoleApplications.Utilities;

namespace Vertical.ConsoleApplications.Providers
{
    public class ProviderBuilder
    {
        internal ProviderBuilder(IServiceCollection applicationServices)
        {
            ApplicationServices = applicationServices;
        }

        /// <summary>
        /// Gets the application services.
        /// </summary>
        public IServiceCollection ApplicationServices { get; }

        /// <summary>
        /// Adds known arguments to the provider order.
        /// </summary>
        /// <param name="args">Arguments to add.</param>
        /// <returns>A reference to this instance.</returns>
        public ProviderBuilder AddArguments(string[] args) =>
            AddArguments(args, "static");

        /// <summary>
        /// Adds entry arguments to the provider order.
        /// </summary>
        /// <param name="args">Arguments to add.</param>
        /// <returns>A reference to this instance.</returns>
        /// <remarks>
        /// This method is simply a synonym for <see cref="AddArguments"/>, and added
        /// for clarity. 
        /// </remarks>
        public ProviderBuilder AddEntryArguments(string[] args) =>
            AddArguments(args, "entry");

        /// <summary>
        /// Adds arguments parsed from the value of an environment variable.
        /// </summary>
        /// <param name="variableKey">The environment variable.</param>
        /// <returns>A reference to this instance.</returns>
        public ProviderBuilder AddEnvironmentVariable(string variableKey)
        {
            var value = Environment.GetEnvironmentVariable(variableKey);
            var args = ArgumentHelpers.SplitFromString(value ?? string.Empty);
            
            return AddArguments(args, $"environment variable {variableKey}");
        }

        /// <summary>
        /// Adds a <see cref="IArgumentsProvider"/> implementation to the provider order.
        /// </summary>
        /// <typeparam name="T">Provider type</typeparam>
        /// <returns>A reference to this instance.</returns>
        public ProviderBuilder AddProvider<T>() where T : class, IArgumentsProvider
        {
            ApplicationServices.AddSingleton<IArgumentsProvider, T>();
            return this;
        }

        /// <summary>
        /// Adds a <see cref="IArgumentsProvider"/> implementation to the provider order.
        /// </summary>
        /// <param name="implementationFactory">
        /// A function that creates the instance using the given <see cref="IServiceProvider"/>
        /// instance.
        /// </param>
        /// <returns>A reference to this instance.</returns>
        public ProviderBuilder AddProvider(Func<IServiceProvider, IArgumentsProvider> implementationFactory)
        {
            ApplicationServices.AddSingleton(implementationFactory);
            return this;
        }

        /// <summary>
        /// Adds the arguments that are found in the lines of a file.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>A reference to this instance.</returns>
        public ProviderBuilder AddScript(string filePath)
        {
            return AddProvider(services => new ScriptFileArgumentsProvider(
                () => Task.FromResult<IEnumerable<string>>(new[] { filePath }),
                services.GetService<ILogger<ScriptFileArgumentsProvider>>()));
        }

        /// <summary>
        /// Adds the arguments that are found in files matched using a base path
        /// and match pattern.
        /// </summary>
        /// <param name="basePath">The base path to search for script files.</param>
        /// <param name="pattern">A glob pattern used to match files.</param>
        /// <returns>A reference to this instance.</returns>
        public ProviderBuilder AddScripts(string basePath, string pattern)
        {
            Task<IEnumerable<string>> GetScriptFiles(ILogger? logger)
            {
                logger?.LogTrace("Discovering scripts in {path}/{pattern}", basePath, pattern);
                
                var matcher = new Matcher().AddInclude(pattern);
                var directoryInfo = new DirectoryInfo(basePath);
                var results = matcher.Execute(new DirectoryInfoWrapper(directoryInfo));

                return Task.FromResult(results.Files.Select(r => r.Path));
            }

            return AddProvider(services => new ScriptFileArgumentsProvider(
                () => GetScriptFiles(services.GetService<ILogger<ProviderBuilder>>()),
                services.GetService<ILogger<ScriptFileArgumentsProvider>>()));
        }

        /// <summary>
        /// Adds arguments that are received through interactive input.
        /// </summary>
        /// <param name="prompt">A delegate that is executed before reading from the console
        /// input adapter</param>
        /// <returns>A reference to this instance.</returns>
        public ProviderBuilder AddInteractiveConsole(Action? prompt = null) => AddProvider(services =>
            new InteractiveArgumentsProvider(
                services.GetRequiredService<IConsoleInputAdapter>(),
                prompt ?? (() => Console.Write("> ")),
                services.GetService<ILogger<InteractiveArgumentsProvider>>()));
        
        private ProviderBuilder AddArguments(string[] args, string context)
        {
            return AddProvider(sp => new StaticArgumentsProvider(
                args,
                context,
                sp.GetService<ILogger<StaticArgumentsProvider>>()));
        }
    }
}