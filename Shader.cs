using Spectre.Tui;
using System.Numerics;
using TermShader.Infrastructure;

using static System.Math;

public sealed class Shader : ShaderBase
{
  readonly static Vector3 _0    = new(0);
  readonly static Vector3 _1    = new(1);
  readonly static Vector3 _255  = new(255);
  readonly static Vector3 _Base = new(-0.7F,-0.2F,0.3F);

  static Color ToColor(Vector3 c) 
  {
    var C=Vector3.Clamp(c,_0,_1)*_255;
    return new((byte)C.X,(byte)C.Y,(byte)C.Z);
  }

  float     _iy  ;
  Vector2   _res ;
  Vector2   _x   ;
  Vector2   _y   ;

  protected override void Setup(int width, int height, double time)
  {
    _res =new(width, height);
    _iy = 1/_res.Y;
    var (c,s) = SinCos(time);
    _x = new((float)+c,(float)+s);
    _y = new((float)-s,(float)+c);
  }

  protected override Color Run(int x, int y)
  {
    Vector2 
      c = new (x,y)
    , p = (c+c-_res)*_iy
    , t
    ;

    Vector3
      C = _0
    , P
    , R = Vector3.Normalize(new(p.X,p.Y,2))
    ;

    float
      z = 0
    , d = 1
    ;

    int 
      i
    ;

    // No comments needed...
    for(i=0; i<49&&z<4&&d>1e-3; ++i) 
    {
      P=z*R;
      P.Z-=3;
      t=new(P.X,P.Z);
      t=new(Vector2.Dot(t,_x),Vector2.Dot(t,_y));
      P.X=t.X;
      P.Z=t.Y;
      t=new(P.X,P.Y);
      t=new(Vector2.Dot(t,_x),Vector2.Dot(t,_y));
      P.X=t.X;
      P.Y=t.Y;
      P*=P;
      d=(float)Sqrt(Sqrt(Vector3.Dot(P,P)))-1;
      z+=d;
    }

    if (z<4) 
      C=.5F*(_1+Vector3.Sin(_Base-new Vector3(i/33F+2*(p.X+p.Y))));

    return ToColor(C);
  }
}