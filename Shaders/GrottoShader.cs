using Spectre.Tui;
using System.Numerics;
using System.Runtime.CompilerServices;
using TermShader.Infrastructure;

using static System.Math;
using static System.Numerics.Vector4;

public sealed class GrottoShader : ShaderBase
{
  float   _F;
  Vector2 _R;
  Vector2 _R2;
  Vector4 _O;

  public override string Name { get; } = "Glowing grotto";

  protected override void Setup(int width, int height, double time)
  {
    _R=new(width, height);
    _R2=.5F*_R;
    _F=(float)(Sin(time)*Sin(1.7*time)*Sin(2.3*time));
    _O=new(0,0,(float)time/23,.2F);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  float G(Vector4 p, float s) 
  {
    var (S,C)=SinCos(s*p);
    p.X=C.Z; p.Y=C.X;p.Z=C.W; p.W=C.Y;
    return Abs(Dot(S,p)-1)/s;
  }

  protected override Color Run(int x, int y)
  {
    Vector4
      o=Zero
    , p=_O
    , _5=new(5)
    , C=new(1.4F,.7F,0,2.1F)
    , R=Vector3.Normalize(new(new Vector2(x,_R.Y-y)-_R2,_R.Y)).AsVector4()
    ;

    float
      d
    , s
    , z=0
    ;

    for(int i=0;i<50;++i)
    {
      s=p.Y+.1F;
      p.Y=Abs(s)-.31F;
      d=.3F*Abs(G(p,8)-G(p,24));
      p=One+Cos(FusedMultiplyAdd(_5,new(p.Z),C));
      z+=d+5E-4F;
      if(s>0) s=1F; else { d=d*d*d; s=.1F; }
      o=FusedMultiplyAdd(new(s*p.W/Max(d,5E-4F)),p,o);
      p=FusedMultiplyAdd(new(z),R,_O);
    }

    o=FusedMultiplyAdd(new((1E3F+1E3F*_F)/(1E-4F+p.AsVector2().Length())),new Vector4(2,1,0,3),o);
    o*=2E-5F;

    return ToColor(TanhApprox(o.AsVector3()));
  }
}