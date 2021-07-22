using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Vertical.ConsoleApplications;

namespace Vertical.ConsoleApps.Example
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await ConsoleHostBuilder
                .CreateDefault(args)
                .UseStartup<Startup>()
                .RunConsoleAsync();
        }
    }
}
