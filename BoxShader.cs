using Spectre.Tui;
using System.Numerics;
using TermShader.Infrastructure;

using static System.Math;
using static System.Numerics.Vector3;

public sealed class BoxShader : ShaderBase
{
  readonly static Vector3 _Base = new(-0.7F,-0.2F,0.3F);

  float     _inv ;
  float     _fad ;
  Vector2   _res ;
  Vector3   _rot ;

  protected override void Setup(int width, int height, double time)
  {
    float
      t=(float)time
    ;
    _res=new(width, height);
    _inv=1/_res.Y;
    _rot=Normalize(Sin(new Vector3(t)+new Vector3(0,1,2)));
    _fad=.5F*Smoothstep(0,2,t);
  }

  protected override Color Run(int x, int y)
  {
    Vector2
      c=new (x,y)
    , p=(c+c-_res)*_inv
    ;

    Vector3
      P
    , R=Normalize(new(p.X,p.Y,2))
    , r=_rot
    ;

    float
      z=0
    , d=1
    ;

    int
      i
    ;

    // No comments needed...
    for(i=0; i<49&&z<4&&d>1e-3; ++i)
    {
      P=z*R;
      P.Z-=3;
      P=Dot(P,r)*r+Cross(P,r);
      P*=P;
      d=(float)Sqrt(Sqrt(Dot(P,P)))-1;
      z+=d;
    }

    return ToColor(
      z<4
      ?_fad*(One+Sin(_Base-new Vector3(i/33F+2*(p.X+p.Y))))
      :Zero
      );
  }
}