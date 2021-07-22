using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vertical.ConsoleApplications.Pipeline;

namespace Vertical.ConsoleApplications
{
    public static class StartupExtensions
    {
        public static IHostBuilder UseStartup<T>(this IHostBuilder hostBuilder) where T : class
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                var provider = new ServiceCollection()
                    .AddSingleton(context.Configuration)
                    .AddSingleton<T>()
                    .BuildServiceProvider();

                var startup = provider.GetRequiredService<T>();
                var binder = default(Binder);
                var parameterModifiers = Array.Empty<ParameterModifier>();

                typeof(T).GetMethod("ConfigureServices",
                        BindingFlags.Public | BindingFlags.Instance,
                        binder,
                        new[] {typeof(IServiceCollection)},
                        parameterModifiers)?
                    .Invoke(startup, new object?[] {services});

                typeof(T).GetMethod("Configure",
                        BindingFlags.Public | BindingFlags.Instance,
                        binder,
                        new[] {typeof(ApplicationBuilder)},
                        parameterModifiers)?
                    .Invoke(startup, new object?[] {new ApplicationBuilder(services)});
            });
            
            return hostBuilder;
        }
    }
}