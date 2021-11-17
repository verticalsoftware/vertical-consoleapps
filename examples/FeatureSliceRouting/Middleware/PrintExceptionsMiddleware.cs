using System;
using System.Threading;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.Pipelines;

namespace FeatureSliceRouting.Middleware
{
    public class PrintExceptionsMiddleware : IPipelineMiddleware<RequestContext>
    {
        /// <inheritdoc />
        public async Task InvokeAsync(RequestContext context, 
            PipelineDelegate<RequestContext> next, 
            CancellationToken cancellationToken)
        {
            try
            {
                await next(context, cancellationToken);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}