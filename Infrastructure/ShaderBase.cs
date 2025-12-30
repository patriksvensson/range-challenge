using Spectre.Tui;
using System.Numerics;

using static System.Math;
using static System.Numerics.Vector3;

namespace TermShader.Infrastructure;

public abstract class ShaderBase
{
    readonly static Vector3 _255  = new(255);

    public static float Smoothstep(float edge0, float edge1, float x)
    {
      float
        t=(float)Clamp((x-edge0)/(edge1-edge0),0,1)
       ;
      return t*t*(3-2*t);
    }

    public static Color ToColor(Vector3 c)
    {
      var C=Clamp(c,Zero,One)*_255;
      return new((byte)C.X,(byte)C.Y,(byte)C.Z);
    }

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