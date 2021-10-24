# Vertical ConsoleApps

A lightweight mini-framework for console applications with command support, dependency injection, and a processing pipeline.

## Overview

This library provides a base upon which to build feature-rich console applications. Instead of introducing exotic patterns, the look and feel of the framework takes its inspiration from ASP.NET Core applications.

Reasons why you amy want to base your application on this library are as follows:

- The application may have different feature branches based on argument inputs
- The implementation needs services provided and controlled by dependency injection
- Input to the application may need to be validated or manipulated

## Quick Glance

The following example shows the basic example of a console application using the library:

```csharp
static Task Main(string[] args)
{
    var hostBuilder = ConsoleHostBuilder
        .CreateDefault()
        .ConfigureServices(services =>
        {
            // Configure services managed by dependency injection
            services.AddLogging(logging => logging
                .AddConsole()
                .AddFilter("Microsoft.*", LogLevel.Warning)
                .AddFilter("Vertical.*", LogLevel.Warning)
                .SetMinimumLevel(LogLevel.Information);
        })
        .ConfigureProviders(providers =>
        {
            // Providers feed arguments to the pipeline.
            // We're adding a provider here that gets interactive input from
            // the user
            providers.AddInteractiveConsole();
        })
        .Configure(pipeline =>
        {
            // The pipeline handles the arguments similar to a web request
            // and is composed in the order we put them in here. Similar to middleware,
            // each pipeline task is responsible for passing on control.

            // If the user types either of these keywords, exit the application
            pipeline.UseExitCommands("exit", "quit");

            // This will simply echo the arguments the user enters in
            // the console until they type an exit command
            pipeline.Use(next => context =>
            {
                Console.WriteLine(string.Join(' ', context.Arguments));
                return next(context);
            });
        });
}
```
