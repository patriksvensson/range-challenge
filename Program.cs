using Spectre.Tui;
using System.Diagnostics;
using TermShader.Infrastructure;

Console.WriteLine("Box or fractal?");
Console.WriteLine("1. A box please");
Console.WriteLine("2. A fractal sounds nice");
Console.Write("Enter your choice (1-2): ");

ShaderBase shader =
  Console.ReadLine() switch
  {
    "1" => new BoxShader()
  , "2" => new ApolloShader()
  , _   => Random.Shared.NextDouble()>.5?new BoxShader():new ApolloShader()
  };


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

        var fps = (int)(1 / elapsed.TotalSeconds);
        ctx.Render(Text.FromMarkup($"FPS: [blue]{fps}[/]"));
        
        ctx.Render(text2, new Rectangle(0, ctx.Viewport.Bottom - 1, text2.GetWidth(), 1));
        ctx.Render(text, new Rectangle(ctx.Viewport.Right - text.GetWidth(), ctx.Viewport.Bottom - 1, text.GetWidth(), 1));
    });

    // Check for exit
    if (Console.KeyAvailable)
    {
        // Escape is the canonical way to exit PC demos
        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
        {
            isRunning = false;
            // Turn on cursor again
            //Console.Write("\x1b[?25h");
            break;
        }
    }
}