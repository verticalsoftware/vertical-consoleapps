using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vertical.ConsoleApplications.Pipeline
{
    internal class MiddlewareFactory : IMiddlewareFactory
    {
        private readonly IEnumerable<IMiddleware> _tasks;

        /// <summary>
        /// Creates a new instance of this type.
        /// </summary>
        /// <param name="tasks">Ordered sequence of tasks.</param>
        public MiddlewareFactory(IEnumerable<IMiddleware> tasks)
        {
            _tasks = tasks;
        }
        
        /// <inheritdoc />
        public PipelineDelegate Create()
        {
            PipelineDelegate invokeAsync = (_, _) => Task.CompletedTask;

            foreach (var task in _tasks.Reverse())
            {
                var previous = invokeAsync;
                
                invokeAsync = (context, cancelToken) => task.InvokeAsync(context, previous, cancelToken);
            }

            return invokeAsync;
        }
    }
}