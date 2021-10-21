using System;
using System.Threading;
using System.Threading.Tasks;
using Vertical.CommandLine;
using Vertical.CommandLine.Configuration;

namespace Vertical.ConsoleApplications.Routing
{
    /// <summary>
    /// Base class for a command handler that parses arguments into an options
    /// type.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    public abstract class CommandArgumentHandler<TOptions> : ICommandHandler where TOptions : class
    {
        private readonly ApplicationConfiguration<TOptions> _argsConfiguration = new();

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="configure">Delegate used to configure the argument parser.</param>
        protected CommandArgumentHandler(Action<ApplicationConfiguration<TOptions>> configure)
        {
            configure(_argsConfiguration);
        }

        /// <summary>
        /// When implemented by a class, handles the implementation of the command.
        /// </summary>
        /// <param name="options">Options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A Task that completes when the operation is finished.</returns>
        protected abstract Task ExecuteAsync(TOptions options, CancellationToken cancellationToken);

        /// <inheritdoc />
        public Task HandleAsync(string[] arguments, CancellationToken cancellationToken)
        {
            var options = CommandLineApplication.ParseArguments<TOptions>(_argsConfiguration, arguments);

            return ExecuteAsync(options, cancellationToken);
        }
    }
}