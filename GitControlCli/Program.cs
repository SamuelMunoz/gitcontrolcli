using CliWrap;
using CliWrap.Buffered;
using Cocona;
using Spectre.Console;

var builder = CoconaApp.CreateBuilder();

var app = builder.Build();

app.AddCommand("run", async () =>
{
    await AnsiConsole.Status().Spinner(Spinner.Known.Dots)
        .StartAsync("Fetching branches to remove...", async ctx =>
        {
            var fetch = await Cli.Wrap("git").WithArguments("remote update --prune").WithValidation(CommandResultValidation.None).ExecuteBufferedAsync();
        
            AnsiConsole.MarkupLine($"[red]{fetch?.StandardError}[/]");
            AnsiConsole.MarkupLine($"[green]Fetching branches completed[/]");
        
            ctx.Status("Cleaning local branches...");
            ctx.Spinner(Spinner.Known.Clock);

            var branch = await Cli.Wrap("powershell")
                .WithArguments(@"-Command ""git branch --v | ? { $_ -match '\[gone\]' } | % { -split $_ | select -First 1 } | % { git branch -D $_ }"" ")
                .WithValidation(CommandResultValidation.None).ExecuteBufferedAsync();
        
            AnsiConsole.MarkupLine($"[red]{branch.StandardError}[/]");
            AnsiConsole.MarkupLine($"[green]Cleaning local branches completed[/]");
        
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold blue]Done![/]");
        });
}).WithDescription("Runs the application");

app.Run();






