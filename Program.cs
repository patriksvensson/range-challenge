using Spectre.Tui;
using System.Diagnostics;

var isRunning = true;
Console.CancelKeyPress += (e, s) =>
{
    s.Cancel = true;
    isRunning = false;
};

using var terminal = Terminal.Create();
var renderer = new Renderer(terminal);
var shader = new BoxShader();
var sw = Stopwatch.StartNew();

var text = Text.FromMarkup("Created by [yellow]Mårten Rånge[/]");
var text2 = Text.FromMarkup("Spectre.Tui");

while (isRunning)
{
    renderer.Draw((ctx, elapsed) =>
    {
        shader.Render(ctx, sw.Elapsed.TotalSeconds);

        var fps = (int)(1 / elapsed.TotalSeconds);
        ctx.Render(Text.FromMarkup($"FPS: [blue]{fps}[/]"));
        
        ctx.Render(text2, new Rectangle(0, ctx.Viewport.Bottom - 1, text2.GetWidth(), 1));
        ctx.Render(text, new Rectangle(ctx.Viewport.Right - text.GetWidth(), ctx.Viewport.Bottom - 1, text.GetWidth(), 1));
    });
    
    // Check for exit
    if (Console.KeyAvailable)
    {
        if (Console.ReadKey(true).Key == ConsoleKey.Q)
        {
            isRunning = false;
        }
    }
}