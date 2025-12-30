using Spectre.Tui;
using System.Numerics;
using TermShader.Infrastructure;

using static System.Math;
using static System.Numerics.Vector3;

public sealed class LandscapeShader : ShaderBase
{
  float     _time;
  Vector2   _res;

  protected override void Setup(int width, int height, double time)
  {
    _time=(float)time;
    _res=new(width, height);
  }

  protected override Color Run(int x, int y)
  {
    float
        d=1
      , z=0
      , a
      , s
      , t=_time
      ;

    Vector2
        r=_res
      , D
      , P
      , C
      , S
      ;

    Vector3
        O=Zero
      , p
      , R=Normalize(new(new Vector2(x,r.Y-y)-new Vector2(.5F,1)*r, r.Y))
      , G=new(0,2,2)
      , Y=new(2,6,0)
      ;

    Matrix3x2
        Z=new(1.2F,1.6F,-1.6F,1.2F,0,0)
      ;

    for(int i=0;i<50&&d>1e-3;++i)
    {
      p=z*R;
      p.Z+=t;
      P=.23F*(new Vector2(p.X,p.Z));
      D=Vector2.Zero;
      s=p.Y+4;
      d=Abs(s)+.6F;
      a=1;

      for(int j=0;j<5;++j)
      {
        (S,C)=Vector2.SinCos(P);
        p=(new Vector3(C.X,S.X,S.X))*(new Vector3(S.Y,C.Y,S.Y));
        D+=new Vector2(p.X,p.Y);
        d-=a*(1+p.Z)/(1+3*Vector2.Dot(D,D));
        P=Vector2.Transform(P,Z);
        a*=.55F;
      }

      if(s>0)
      {
        O+=One+d*G;
      }
      else
      {
        O+=Y;
      }

      z+=d;
    }

    return ToColor(TanhApprox(O*1E-2F));
  }
}