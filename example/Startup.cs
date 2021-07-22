using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vertical.ConsoleApplications;
using Vertical.ConsoleApplications.Pipeline;

namespace Vertical.ConsoleApps.Example
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureProviders(builder => builder
                .AddEntryArguments()
            );
        }

        public void Configure(ApplicationBuilder app)
        {
            
        }
    }
}