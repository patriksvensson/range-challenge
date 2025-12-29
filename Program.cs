using Spectre.Tui;
using System.Diagnostics;

var terminal = Terminal.Create();
var renderer = new Renderer(terminal);
var shader = new Shader();
var sw = Stopwatch.StartNew();

while (true)
{
    renderer.Draw((ctx, elapsed) =>
    {
        shader.Render(ctx, sw.Elapsed.TotalSeconds);
    });
    
    // Check for exit
    if (Console.KeyAvailable)
    {
        if (Console.ReadKey(true).Key == ConsoleKey.Q)
        {
            break;
        }
    }
}