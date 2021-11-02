using System;
using System.Threading;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Pipeline;

namespace FeatureSliceRouting.Middleware
{
    public class PrintExceptionsMiddleware : IMiddleware
    {
        /// <inheritdoc />
        public async Task InvokeAsync(RequestContext context, 
            PipelineDelegate next, 
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