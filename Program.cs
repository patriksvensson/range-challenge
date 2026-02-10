using Spectre.Tui;
using System.Diagnostics;
using Spectre.Console;
using TermShader.Infrastructure;
using Text = Spectre.Tui.Text;

var shader = new SelectionPrompt<ShaderBase>()
    .Title("[yellow]What do you want to run?[/]")
    .UseConverter(c => c.Name)
    .AddChoices(
        new BoxShader(),
        new ApolloShader(),
        new LandscapeShader(),
        new GrottoShader())
    .Show(AnsiConsole.Console);

var isRunning = true;
Console.CancelKeyPress += (e, s) =>
{
    s.Cancel = true;
    isRunning = false;
};

using var terminal = Terminal.Create();
var renderer = new Renderer(terminal);
var sw = Stopwatch.StartNew();

var text = Text.FromMarkup("Created by [yellow]Mårten Rånge[/]");
var text2 = Text.FromMarkup("Spectre.Tui");

while (isRunning)
{
    renderer.Draw((ctx, elapsed) =>
    {
        shader.Render(ctx, sw.Elapsed.TotalSeconds);
        
        ctx.Render(Text.FromMarkup($"FPS: [blue]{(int)(1 / elapsed.TotalSeconds)}[/]"));
        ctx.Render(text2, new Rectangle(0, ctx.Viewport.Bottom - 1, text2.GetWidth(), 1));
        ctx.Render(text, new Rectangle(ctx.Viewport.Right - text.GetWidth(), ctx.Viewport.Bottom - 1, text.GetWidth(), 1));
    });
    
    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey(true).Key; 
        if (key is ConsoleKey.Escape or ConsoleKey.Q)
        {
            isRunning = false;
        }
    }
}