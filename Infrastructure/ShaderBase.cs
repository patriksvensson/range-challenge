using Spectre.Tui;

namespace TermShader.Infrastructure;

public abstract class ShaderBase
{
    public void Render(RenderContext context, double time)
    {
        int width = context.Viewport.Width;
        int height = context.Viewport.Height;

        Setup(width, height+height, time);

        // Not sure if Spectre handles parallel assignments?
        //  At least we won't modify the same cell from multiple threads
        Parallel.For(0, width, x => 
        {
            for (var y = 0; y < height; y++)
            {
                var c = context.GetCell(x,y);
                if (c is not null)
                {
                  // Resolution doubler: U+2580
                  c.SetSymbol('▀');
                  c.SetForeground(Run(x, y+y+0));
                  c.SetBackground(Run(x, y+y+1));
                }
            }
        });
    }

    protected abstract void Setup(int width, int height, double time);
    protected abstract Color Run(int x, int y);
}