using Spectre.Tui;
using System.Numerics;
using TermShader.Infrastructure;

using static System.MathF;
using static System.Numerics.Vector3;

public sealed class ApolloShader : ShaderBase
{
  readonly static Vector3 _Base = new(2,1,0);

  float   _inv;
  float   _fad;
  float   _sin;
  Vector2 _res;
  Vector3 _rot;

  public override string Name { get; } = "Fractals";

  protected override void Setup(int width, int height, double time)
  {
    var t=(float)time;
    _res=new(width, height);
    _inv=1/_res.Y;
    _rot=Normalize(Sin(new Vector3(.2F*t+123)+new Vector3(0,1,2)));
    _sin=Sin(.123F*t);
    _fad=.5F;
  }

  protected override Color Run(int x, int y, Color previous)
  {
    Vector2
      c=new (x,y)
    , p=(c+c-_res)*_inv
    ;

    Vector3
      P=new(p.X,p.Y,.5F*_sin)
    ;

    float
      s=1
    , k
    ;

    P=Dot(P,_rot)*_rot+Cross(P,_rot);

    for(int i=0; i<3;++i)
    {
      P-=2F*Round(.5F*P);
      k=1.41F/Dot(P,P);
      P*=k;
      s*=k;
    }

    P=Abs(P)/s;
    k=Min(P.Z, new Vector2(P.X,P.Y).Length());

    return ToColor(
        k<5E-3F
      ? One
      : _fad/(1+k*k*5)*(One+Sin(_Base+new Vector3(2+Log2(k))))
      );
  }
}