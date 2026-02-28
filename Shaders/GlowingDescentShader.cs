using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using TermShader.Infrastructure;

using static System.MathF;
using static System.Numerics.Vector3;

public sealed class GlowingDescentShader : ShaderBase
{
  public override string Name { get; } = "Glowing Descent";

  static readonly Vector2 _P=new(.4F,.3F);
  static readonly Vector4 _M=Vector128.Create(0,0,~0,~0).AsSingle().AsVector4();

  Vector2 _R;
  Vector3 _S;
  Vector3 _X;
  Vector3 _Y;
  Vector3 _Z;

  protected override void Setup(int width, int height, double time)
  {
    var t=(float)time;
    _R=new(width,height);
    Vector3
      o0
    , o1
    , o2
    ;
    O3(t,out o0, out o1, out o2);
    _S=o0;
    _Z=Normalize(o1);
    _X=Normalize(Cross(new Vector3(0,1,0)+o2,_Z));
    _Y=Cross(_X,_Z);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  static void O3(float z, out Vector3 o0, out Vector3 o1, out Vector3 o2)
  {
    Vector2 
      p=_P
    , c=Vector2.Cos(p*z)
    , s=Vector2.Sin(p*z)
    ;
    o0=new(s,z);
    o1=new(p*c,1);
    o2=new(p*p*s,0);
  }

  protected override Color Run(int x, int y, Color previous)
  {
    float
      d
    , z=0
    ;
    Vector2
      C=new(x,_R.Y-y)
    ;

    Vector3
      p
    , P
    , I
    , S=_S
    , Z
    , X
    ;

    Vector4
      o=Vector4.Zero
    , O
    , M=_M
    , Q
    ;

    C+=C-_R;

    I=Normalize(C.Y*_Y+2*_R.Y*_Z-C.X*_X);

    for(
      int i=0
    ; i<77
    ; ++i
    )
    {
      p=FusedMultiplyAdd(new(z),I,S);
      O3(p.Z,out P,out Z,out X);
      p-=new Vector3(P.X, P.Y, 0);
      Z=Normalize(Z);
      p-=Dot(new Vector3(p.X, p.Y, 0),Z)*new Vector3(.5F,.5F,-.5F)*Z;
      O=Cos(new Vector3(X.X)+new Vector3(0,11,33)).AsVector4();
      Q=p.AsVector4();
      Q=Vector4.FusedMultiplyAdd(Vector4.Shuffle(O,2,1,3,3),Vector4.Shuffle(Q,1,0,2,3),Vector4.FusedMultiplyAdd(Vector4.Shuffle(O,0,0,3,3),Q,Vector4.BitwiseAnd(Q,M)));
      p=Q.AsVector3();
      P=p;
      p-=Round(p);
      p*=p;
      z+=d=1E-3F+.7F*Abs(.53F-Sqrt(Sqrt(Dot(p,p))));
      O=new Vector4(.8F)+Vector4.Sin(new Vector4(2*Abs(P.X))+new Vector4(8,3,4,2));
      o+=O.W/d*O;
    }

    o*=5E-5F;

    return ToColor(TanhApprox(o.AsVector3()));
  }
}