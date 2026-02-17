using Spectre.Tui;
using System.Numerics;
using System.Runtime.Intrinsics;
using TermShader.Infrastructure;

using static System.MathF;
using static System.Numerics.Vector3;

public sealed class NothingSpecialShader : ShaderBase
{
  public override string Name { get; } = "Nothing Special";

  Vector2 _R;
  Vector2 _R2;
  Vector3 _O;

  protected override void Setup(int width, int height, double time)
  {
    _O=new(0,0,(float)time);
    _R=new(width, height);
    _R2=.5F*_R;
  }

  protected override Color Run(int x, int y, Color previous)
  {
    float
      d
    , z=0
    ;

    Vector3
      p
    , X
    , o=Zero
    , R=Normalize(new(new Vector2(x,y)-_R2,_R.Y))
    , O=_O
    ;

    Vector4
      U=new(2,1,0,3)
    , Y
    , e
    , M=Vector128.Create(0,0,~0,~0).AsSingle().AsVector4()
    ;

    for(int i=0;i<66;++i)
    {
      p=FusedMultiplyAdd(new(z),R,O);
      X=p;
      Y=Cos(new Vector3(.4F*p.Z)+new Vector3(0,11,33)).AsVector4();
      e=p.AsVector4();
      e=Vector4.FusedMultiplyAdd(Vector4.Shuffle(Y,2,1,3,3),Vector4.Shuffle(e,1,0,2,3),Vector4.FusedMultiplyAdd(Vector4.Shuffle(Y,0,0,3,3),e,Vector4.BitwiseAnd(e,M)));
      p=e.AsVector3();
      p-=new Vector3(.5F);
      p-=Round(p);
      p=(p*p)*(p*p);
      d=FusedMultiplyAdd(.6F,Abs(Sqrt(Sqrt(Sqrt(Dot(p,p))))-.3F),1E-3F);
      z+=d;
      o=FusedMultiplyAdd(new(1/d),new Vector3(1.1F)+Sin(new Vector3(FusedMultiplyAdd(2F,X.AsVector2().Length(),.5F*X.Z))+U.AsVector3()),o);
    }

    o*=1E-4F;

    return ToColor(TanhApprox(o));
  }
}