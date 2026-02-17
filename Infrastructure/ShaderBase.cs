using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using static System.Numerics.Vector3;

namespace TermShader.Infrastructure;

public abstract class ShaderBase
{
    readonly static Vector3 _27   = new(27);
    readonly static Vector3 _255  = new(255);
    readonly static Vector3 _i255 = new(1F/255);
    
    public abstract string Name { get; }

    public static float Smoothstep(float edge0, float edge1, float x)
    {
      float
        t=(float)Math.Clamp((x-edge0)/(edge1-edge0),0,1)
       ;
      return t*t*(3-2*t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Fract(float x)
    {
      return x-MathF.Floor(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Floor(Vector3 x)
    {
      return Vector128.Floor(x.AsVector128()).AsVector3();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Fract(Vector3 x)
    {
      return x-Floor(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color ToColor(Vector3 c)
    {
      var C=Clamp(c,Zero,One)*_255;
      return new((byte)C.X,(byte)C.Y,(byte)C.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 FromColor(Color c)
    {
      return new Vector3(c.R,c.G,c.B)*_i255;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 TanhApprox(Vector3 x)
    {
      Vector3
        x2=x*x
      ;
      return Clamp(x*(_27+x2)/(9*x2+_27),-One,One);
    }

    Color[] _backBuffer=[];

    public void Render(RenderContext context, double time)
    {
        int width  = context.Viewport.Width;
        int height = context.Viewport.Height;
        int total  = width*height*2;

        if(_backBuffer.Length != total)
        {
            _backBuffer=new Color[total];
        }

        Setup(width, height+height, time);

        // Not sure if Spectre handles parallel assignments?
        //  At least we won't modify the same cell from multiple threads
        Parallel.For(0, height, y =>
        {
            var yoff = 2*y*width;
            for (var x = 0; x < width; x++)
            {
                var xoff=yoff+x;
                var c = context.GetCell(x,y);
                if (c is not null)
                {
                  // Resolution doubler: U+2580
                  c.SetSymbol('\x2580');
                  _backBuffer[xoff] = Run(x, y+y+0, _backBuffer[xoff]);
                  _backBuffer[xoff+width] = Run(x, y+y+1, _backBuffer[xoff+width]);
                  c.SetForeground(_backBuffer[xoff]);
                  c.SetBackground(_backBuffer[xoff+width]);
                }
            }
        });
    }

    protected abstract void Setup(int width, int height, double time);
    protected abstract Color Run(int x, int y, Color previous);
}