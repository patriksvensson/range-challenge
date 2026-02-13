using System.Diagnostics;
using System.Runtime.CompilerServices;
using TermShader;
using TermShader.Infrastructure;

public static class Program
{
    private static void Main(string[] args)
    {
        var cst = new CancellationTokenSource();
        Console.CancelKeyPress += (e, s) =>
        {
            s.Cancel = true;
            cst.Cancel();
        };

        using var terminal = Terminal.Create();
        var renderer = new Renderer(terminal);
        renderer.SetTargetFps(60);

        // Ask the user to select a shader
        var shader = ShaderSelector.Select(renderer, cst.Token);
        if (shader == null)
        {
            return;
        }

        // Run the selected shader
        Run(renderer, shader, cst.Token);
    }

    private static void Run(Renderer renderer, ShaderBase shader, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        while (!cancellationToken.IsCancellationRequested)
        {
            renderer.Draw((ctx, elapsed) =>
            {
                shader.Render(ctx, sw.Elapsed.TotalSeconds);

                // Render FPS
                var fps = (int)(1 / elapsed.TotalSeconds);
                ctx.Render(Text.FromString($"{fps}", new Style
                {
                    Foreground = GetFpsColor(fps),
                }));
            });

            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key is ConsoleKey.Escape or ConsoleKey.Q)
                {
                    return;
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Color GetFpsColor(int fps)
    {
        return fps switch
        {
            >= 59 => Color.Green,
            >= 24 => Color.Yellow,
            _ => Color.Red,
        };
    }
}