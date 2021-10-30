using System;
using System.Threading;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;

namespace HandlerCommandRouting
{
    public class HelpHandler : ICommandHandler
    {
        /// <inheritdoc />
        [Command("help")]
        public Task HandleAsync(CommandContext context, CancellationToken cancellationToken)
        {
            Console.WriteLine("This program demonstrates inline routed commands by pretending to be a calculator.");
            Console.WriteLine("Type a command (add, sub, mul, div, pow, min, max) and two operands - e.g. add 2 5");
            Console.WriteLine("Type 'quit' or 'exit' to stop the application.");
            
            return Task.CompletedTask;
        }
    }
}