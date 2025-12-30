using Spectre.Tui;
using System.Diagnostics;
using TermShader.Infrastructure;

ShaderBase? shader=null;
while(shader is null)
{
    Console.WriteLine("What do you want?");
    Console.WriteLine("1. A box please");
    Console.WriteLine("2. A fractal sounds nice");
    Console.WriteLine("3. Show me a landscape");
    Console.Write("Enter your choice (1-3): ");

    shader =
      Console.ReadLine() switch
      {
        "1" => new BoxShader()
      , "2" => new ApolloShader()
      , "3" => new LandscapeShader()
      , _   => null
      };
}

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
    
    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey(true).Key; 
        if (key is ConsoleKey.Escape or ConsoleKey.Q)
        {
            isRunning = false;
        }
    }
}