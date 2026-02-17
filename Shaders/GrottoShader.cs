using Spectre.Tui;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using TermShader.Infrastructure;

using static System.MathF;
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
    var t=(float)time;
    _R=new(width, height);
    _R2=.5F*_R;
    _F=(Sin(t)*Sin(1.7F*t)*Sin(2.3F*t));
    _O=new(0,0,t/23,.2F);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  float G(Vector4 p, float s) 
  {
    var (S,C)=SinCos(s*p);
    return Abs(Dot(S,Shuffle(C,2,0,3,1))-1)/s;
  }

  protected override Color Run(int x, int y, Color previous)
  {
    Vector4
      o=Zero
    , p=_O
    , _5=new(5)
    , U=new(2,1,0,3)
    , C=.7F*U
    , R=Vector3.Normalize(new(new Vector2(x,_R.Y-y)-_R2,_R.Y)).AsVector4()
    , X
    , M=Vector128.Create(0,0,~0,~0).AsSingle().AsVector4()
    ;

    float
      d
    , s
    , z=0
    ;

    for(int i=0;i<50;++i)
    {
      s=p.Y+.1F;
      p.Y=Abs(s)-.11F;
      X=Vector3.Cos(new Vector3(p.Z+p.Z)+new Vector3(0,11,33)).AsVector4();
      p=FusedMultiplyAdd(Shuffle(X,2,1,3,3),Shuffle(p,1,0,2,3),FusedMultiplyAdd(Shuffle(X,0,0,3,3),p,BitwiseAnd(p,M)));
      p.Y-=.2F;
      d=.3F*Abs(G(p,8)-G(p,24));
      p=One+Cos(FusedMultiplyAdd(_5,new(p.Z),C));
      z+=d+5E-4F;
      if(s>0) s=1F; else { d=d*d*d; s=.1F; }
      o=FusedMultiplyAdd(new(s*p.W/Max(d,5E-4F)),p,o);
      p=FusedMultiplyAdd(new(z),R,_O);
    }

    o=FusedMultiplyAdd(new(FusedMultiplyAdd(1E3F,_F,1E3F)/(1E-4F+p.AsVector2().Length())),U,o);
    o*=2E-5F;

    return ToColor(TanhApprox(o.AsVector3()));
  }
}