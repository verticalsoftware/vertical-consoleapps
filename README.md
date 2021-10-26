# Vertical ConsoleApps

A lightweight mini-framework for console applications with command support, dependency injection, and a processing pipeline.

## Overview

This library provides a base upon which to build feature-rich console applications using `IHostedService`, and provides an environment integrated with dependency injection and a configurable command pipeline.

### Features

1. Route control flow to different methods depending on command arguments.
2. Fully integrate services with dependency injection.
3. Use API style "controllers" to handle command arguments.
4. Validate, transform, or even short circuit a fully customizable argument pipeline.

### Design Overview

The framework is divided into two main components. _Providers_ feed commands to the application (think of this as making requests), then the _command pipeline_ is responsible for getting the commands to some type of handler (think of this as the response). Providers can be the program's arguments from `Main`, commands read from script files, interactive user console input, etc. The command pipeline can validate and manipulate the commands and arguments, and pass control to logic that responds to the commands. All of this functionality is built on the implementation of `IHostedService`.

![design](assets/flow.png)

## Usage

### Installation

Install the package into a console application.

```
$ dotnet add package vertical-consoleapps
```

### Skeleton Setup

At a minimum, the application will need at least one provider and then one handler for provider arguments. The example below sets up an application that simply prints the entry arguments:

```csharp
static Task Main(string[] args)
{
    var hostBuilder = ConsoleHostBuilder.CreateDefault(builder =>
    {
        // Adds the program's entry arguments
        builder.ConfigureProviders(providers => providers.AddArguments(args));

        // Setup the command pipeline to just print the arguments received
        builder.Configure(app =>
        {
            app.Use((context, next, cancelToken) =>
            {
                Console.WriteLine(context.OriginalFormat);
            });
        });

        return hostBuilder.RunConsoleAsync();
    });
}
```

### Adding other providers

Other command providers can be added in the `ConfigureProviders` method. As stated earlier, providers feed commands to the command pipeline for handling. The providers are executed in the order they are registered in the delegate.

```csharp
builder.ConfigureProviders(providers =>
{
    // Manually add commands
    providers.AddArguments(...);

    // Read commands from a script file. Each line from the file is read
    // and then split into arguments
    providers.AddScript(path);

    // Or a series of files in a base path that match a glob pattern
    providers.AddScript(path, globPattern);

    // Use environment variable
    providers.AddEnvironmentVariable("STARTUP_ARGS");

    // Let user type commands with a custom prompt
    providers.AddInteractiveConsole(() => Console.Write("Enter command > "));
});
```

### Argument Pipeline

The argument pipeline executes with arguments received from the providers. It is a pipeline because it is composed of an ordered set of middleware components that can act or even manipulate the command arguments. The pipeline is composed using the `Configure` method of the host builder. Each component in the pipeline passes control to the next component.

```csharp
builder.Configure(app =>
{
    // Out-of-box middleware that will stop the application if the following
    // commands are encountered
    app.UseExitCommands("quit", "exit");

    // Replace environment variable tokens in any command argument, like
    // $PATH or $USER/$USERNAME
    app.UseEnvironmentVariableTokens();

    // Handle the arguments directly in the pipeline.
    app.Use(async (context, next, cancelToken) =>
    {
        var args = context.Arguments;

        // Do something asynchronous with args

        // Pass on control flow
        await next(context, cancelToken);
    });
});
```

The pipeline can be _short-circuited_ by not passing on control flow to the `next` delegate. This is how the exit command middleware works.

```csharp
builder.Configure(app =>
{
    app.Use(async (context, next, cancelToken) =>
    {
        if (context.Arguments.FirstOrDefault() == "quit")
        {
            context.ApplicationLifetime.StopApplication();
            return;
        }

        await next(context, cancelToken);
    });
});
```

### Command Pattern

The command pattern is a method of evaluating the arguments and distinguishing between a course of action and data that affect how the action is executed. Consider the following input to the dotnet CLI:

```
$ dotnet build -c release --no-restore
```

While `dotnet` is the name of program being executed, `build` is the command. The arguments that follow support the command. Supporting multiple _commands_ is where the utility of this framework is the most compelling. Action can be routed by distinguishing commands from behavioral options. Commands can also be composed of multiple verbs.

#### Delegate handler mapping

Commands can be mapped to delegates using routing services. Routing services break apart the arguments and determine if there are any matches to the arguments, starting with the first argument in the array. If the leading arguments match the mapped value, the delegate is invoked. The example below demonstrates how to map a command to a delegate.

```csharp
builder
    // Add command routing services
    .ConfigureServices(services => services.AddCommandRouting());
    
    // Map delegates using command routing
    .Configure(app => 
    {
        app.UseRouting(router =>
        {
            // Example match: > backup ~/documents/file.txt ~/documents/file.backup.txt
                        
            router.Map("backup", (context, cancelToken) => 
            {
                var src = context.Arguments[0];
                var dest = context.Arguments[1];
                
                File.Copy(src, dest);
                
                return Task.CompletedTask;
            });
            
            // Example match: > list files c:\Users\me\MyDocuments
            
            router.Map("list files", (context, cancelToken) =>
            {
                foreach(var item in Directory.GetFiles(context.Arguments[0]))
                {
                    Console.WriteLine(item);
                }
            };
        });
    });
```

#### Handlers

A handler is an implementation of the `ICommandHandler` interface, and is designed to receive the command context from the pipeline and react accordingly. The benefit of implementing command logic in a class is that the handler can have access to dependency injected services. The example below moves implementation of `backup` command to a handler.

```csharp
public class BackupFeatureHandler : ICommandHandler
{
    private readonly ILogger _logger;

    public BackupFeatureHandler(ILogger<BackupFeatureHandler> logger) => _logger = logger;
    
    public Task HandleAsync(CommandContext context, CancellationToken cancelToken)
    {
        var src = context.Arguments[0];
        var dest = context.Arguments[1];

        _logger.LogInformation("Copy file {src} to {dest}", src, dest);

        File.Copy(src, dest);

        return Task.Completed;
    }     
}

// Configuration
builder.Configure(app =>
{
    app.UseRouting(router =>
    {
        router.MapHandler<BackupFeatureHandler>("backup");
    });
});
```

Multiple route handlers can be defined in a class. To use this feature, do not use the `ICommandHandler` interface. Instead, decorate each handler method with `CommandAttribute`.

```csharp
public class FileFeatureHandler
{
    [Command("backup")]
    public Task BackupFileAsync(CommandContext context, CancellationToken cancelToken)
    {
        File.Copy(context.Arguments[0], context.Argument[1]);
        return Task.CompletedTask;
    }
    
    [Command("delete")]
    public Task DeleteFileAsync(CommandContext context, CancellationToken cancelToken)
    {
        File.Delete(context.Arguments[0]);
        return Task.CompletedTask;
    } 
}

// Configuration
builder.Configure(app =>
{
    app.UseRouting(router =>
    {
        router.MapHandler<FileFeatureHandler>();
    });
});
```