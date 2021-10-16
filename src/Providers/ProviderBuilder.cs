using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Extensions.Logging;

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
        public ProviderBuilder AddArguments(IReadOnlyList<string> args)
        {
            return AddProvider(sp => new StaticArgumentsProvider(
                args,
                sp.GetService<ILogger<StaticArgumentsProvider>>()));
        }

        /// <summary>
        /// Adds entry arguments to the provider order.
        /// </summary>
        /// <param name="args">Arguments to add.</param>
        /// <returns>A reference to this instance.</returns>
        /// <remarks>
        /// This method is simply a synonym for <see cref="AddArguments"/>, and added
        /// for clarity. 
        /// </remarks>
        public ProviderBuilder AddEntryArguments(IReadOnlyList<string> args) =>
            AddArguments(args);

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
            return AddProvider(sp => new ScriptFileArgumentsProvider(
                () => Task.FromResult<IEnumerable<string>>(new[] { filePath }),
                sp.GetService<ILogger<ScriptFileArgumentsProvider>>()));
        }

        /// <summary>
        /// Adds the arguments that are found in files matched using a base path
        /// and glob pattern.
        /// </summary>
        /// <param name="basePath">The base path to search for script files.</param>
        /// <param name="pattern">A glob pattern used to match files.</param>
        /// <returns>A reference to this instance.</returns>
        public ProviderBuilder AddScriptsPath(string basePath, string pattern)
        {
            Task<IEnumerable<string>> GetScriptFiles()
            {
                var matcher = new Matcher().AddInclude(pattern);
                var directoryInfo = new DirectoryInfo(basePath);
                var results = matcher.Execute(new DirectoryInfoWrapper(directoryInfo));

                return Task.FromResult(results.Files.Select(r => r.Path));
            }

            return AddProvider(services => new ScriptFileArgumentsProvider(
                GetScriptFiles,
                services.GetService<ILogger<ScriptFileArgumentsProvider>>()));
        }

        /// <summary>
        /// Adds arguments that are received through interactive input.
        /// </summary>
        /// <returns>A reference to this instance.</returns>
        public ProviderBuilder AddInteractiveConsole() => AddProvider<InteractiveArgumentsProvider>();
    }
}