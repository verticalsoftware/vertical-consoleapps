using System.Threading;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Routing
{
    /// <summary>
    /// Represents the interface of an object that handles the logic of a
    /// command.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// When implemented by a class, performs the application logic for
        /// the command.
        /// </summary>
        /// <param name="arguments">
        /// The arguments contextual to the command (command removed).
        /// </param>
        /// <param name="cancellationToken">
        /// A token that cna be observed for cancellation requests.
        /// </param>
        /// <returns>Task that completes when the command finished execution.</returns>
        Task HandleAsync(string[] arguments, CancellationToken cancellationToken);
    }
}