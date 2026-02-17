using Spectre.Tui;
using System.Numerics;
using TermShader.Infrastructure;

using static System.MathF;
using static System.Numerics.Vector3;

public sealed class StarFieldShader : ShaderBase
{
  public override string Name { get; } = "Star field";

  float _z;

  Vector2 _P;
  Vector2 _R;
  Vector2 _R2;

  Vector3 _O;
  Vector3 _X;
  Vector3 _Y;
  Vector3 _Z;

  Vector3 _RX;
  Vector3 _RY;
  Vector3 _RZ;


  protected override void Setup(int width, int height, double time)
  {
    var t=(float)time;
    _z=2*t;
    _P=new Vector2(1,.707F)/9;
    _R=new (width,height);
    _R2=.5F*_R;
    _Z=Normalize(new(2*_P*Vector2.Cos(_P*_z), 1));
    _X=new(-_Z.Z,0,_Z.X);
    _Y=Cross(_X,_Z);
    _O=new(2*Vector2.Sin(_z*_P),_z);
    _RX=new Vector3(13,78,38);
    _RY=new Vector3(39,68,23);
    _RZ=new Vector3(48,82,16);
  }

  protected override Color Run(int x, int y, Color previous)
  {
    Vector2
      C=new Vector2(x,y)-_R2
    ;

    Vector3
      I=Normalize(C.Y*_Y-C.X*_X+_R.Y*_Z)
    , O=_O
    , o=new Vector3(4,12,36)/(1.002F-I.Z)
    , U=new(0,1,2)
    , p
    , N
    ;

    float
      IZ=1/I.Z
    , z=Fract(-_z)*IZ
    ;

    for(int i=0;i<13;++i) 
    {
      p=FusedMultiplyAdd(new(z),I,O);
      N=Round(p);
      p-=N;
      N=Fract(9E3F*Vector3.Sin(new(Dot(N,_RX),Dot(N,_RY),Dot(N,_RZ))))-new Vector3(.5F);
      C=(p-.8F*N).AsVector2();
      o=FusedMultiplyAdd(new(ReciprocalEstimate(Exp(z*z/33)*Vector2.Dot(C,C))), One+Sin(new Vector3(9*N.Z)+U), o);
      z+=IZ;
    }

    o=FusedMultiplyAdd(new(3E-4F),o,-1e-2F*new Vector3(3,9,1));
    o=Max(o,Zero);
    o=TanhApprox(o);
    o=SquareRoot(o);
    o=Lerp(FromColor(previous),o,FusedMultiplyAdd(.8F,Dot(new(.299F,.587F,.114F),o),.2F));

    return ToColor(o);
  }
}