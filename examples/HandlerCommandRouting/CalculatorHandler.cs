using System;
using System.Threading;
using System.Threading.Tasks;
using Vertical.ConsoleApplications.Pipeline;
using Vertical.ConsoleApplications.Routing;

namespace HandlerCommandRouting
{
    [CommandHandler]
    public class CalculatorHandler
    {
        [Command("add")]
        public Task AddAsync(CommandContext context, CancellationToken cancelToken) =>
            PrintResult(context, (a, b) => a + b);
        
        [Command("sub")]
        public Task SubtractAsync(CommandContext context, CancellationToken cancelToken) =>
            PrintResult(context, (a, b) => a - b);
        
        [Command("mul")]
        public Task MultiplyAsync(CommandContext context, CancellationToken cancelToken) =>
            PrintResult(context, (a, b) => a * b);

        [Command("div")]
        public Task DivideAsync(CommandContext context, CancellationToken cancelToken) =>
            PrintResult(context, (a, b) => a / b);
        
        [Command("pow")]
        public Task RaiseAsync(CommandContext context, CancellationToken cancelToken) =>
            PrintResult(context, Math.Pow);
        
        [Command("min")]
        public Task GetMinAsync(CommandContext context, CancellationToken cancelToken) =>
            PrintResult(context, Math.Min);
        
        [Command("max")]
        public Task GetMaxAsync(CommandContext context, CancellationToken cancelToken) =>
            PrintResult(context, Math.Max);

        
        private static Task PrintResult(CommandContext context, Func<double, double, double> op)
        {
            var args = context.Arguments;
            var operands = Parse(args);
            var result = op(operands.a, operands.b);
            Console.WriteLine(result);

            return Task.CompletedTask;
        }

        private static (double a, double b) Parse(string[] args)
        {
            if (args.Length != 2)
            {
                throw new ApplicationException("Two operands are required");
            }

            if (!double.TryParse(args[0], out var a))
            {
                throw new ApplicationException($"Cannot convert '{args[0]}' to a number.");
            }
            
            if (!double.TryParse(args[1], out var b))
            {
                throw new ApplicationException($"Cannot convert '{args[1]}' to a number.");
            }

            return (a, b);
        }
    }
}