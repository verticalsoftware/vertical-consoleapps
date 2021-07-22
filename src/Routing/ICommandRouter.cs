using System.Threading;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Routing
{
    public interface ICommandRouter
    {
        Task<bool> TryRouteCommandAsync(string[] arguments, CancellationToken cancellationToken);
    }
}