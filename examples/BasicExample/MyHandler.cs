using System;
using System.Threading;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Routing;

namespace BasicExample
{
    [CommandHandler("backup")]
    public class MyHandler : ICommandHandler
    {
        /// <inheritdoc />
        public Task HandleAsync(string[] arguments, CancellationToken cancellationToken)
        {
            Console.WriteLine("In handler! Arguments = {0}", string.Join(" ", arguments));
            return Task.CompletedTask;
        }
    }
}