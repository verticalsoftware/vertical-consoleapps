using System;
using System.Threading;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Routing;
using Vertical.Pipelines;

namespace Vertical.ConsoleApplications.Pipeline
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
                throw new InvalidOperationException("Could not route");
            }

            await next.InvokeAsync(context, cancellationToken);
        }
    }
}