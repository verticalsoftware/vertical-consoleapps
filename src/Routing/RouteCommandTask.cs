using System.Threading;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Routing
{
    public class RouteCommandTask : IPipelineTask<CommandContext>
    {
        private readonly ICommandRouter commandRouter;

        public RouteCommandTask(ICommandRouter commandRouter)
        {
            this.commandRouter = commandRouter;
        }
        
        /// <inheritdoc />
        public async Task InvokeAsync(CommandContext context, 
            PipelineDelegate<CommandContext> next, 
            CancellationToken cancellationToken)
        {
            var routed = await commandRouter.TryRouteCommandAsync(context.Arguments, cancellationToken);

            if (!routed)
            {
                throw new RouteNotFoundException(context.Arguments);
            }

            await next.InvokeAsync(context, cancellationToken);
        }
    }
}