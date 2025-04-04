namespace BrawlhallaAnimLib.Math;

/*
Transform matrix layout:
ScaleX      SkewX       TranslateX
SkewY       ScaleY      TranslateY
0           0           1

SkewX = RotateSkew1
SkewY = RotateSkew0
*/

public readonly record struct Transform2D(double ScaleX, double SkewX, double SkewY, double ScaleY, double TranslateX, double TranslateY)
{
    public static readonly Transform2D IDENTITY = new(1, 0, 0, 1, 0, 0);
    public static readonly Transform2D ZERO = new(0, 0, 0, 0, 0, 0);
    public static readonly Transform2D FLIP_X = new(-1, 0, 0, 1, 0, 0);
    public static readonly Transform2D FLIP_Y = new(1, 0, 0, -1, 0, 0);

    public static Transform2D CreateTranslate(double x, double y) => IDENTITY with { TranslateX = x, TranslateY = y };
    public static Transform2D CreateScale(double scaleX, double scaleY) => IDENTITY with { ScaleX = scaleX, ScaleY = scaleY };
    public static Transform2D CreateSkew(double skewX, double skewY) => IDENTITY with { ScaleX = System.Math.Cos(skewY), SkewX = -System.Math.Sin(skewX), SkewY = System.Math.Sin(skewY), ScaleY = System.Math.Cos(skewX) };
    public static Transform2D CreateRotate(double rot) => CreateSkew(rot, rot);
    public static Transform2D CreateFrom(double x = 0, double y = 0, double rot = 0, double skewX = 0, double skewY = 0, double scaleX = 1, double scaleY = 1) =>
        CreateTranslate(x, y) *
        CreateRotate(rot) *
        CreateSkew(skewX, skewY) *
        CreateScale(scaleX, scaleY);

    public static Transform2D operator *(Transform2D t1, Transform2D t2) => new(
        t1.ScaleX * t2.ScaleX + t1.SkewX * t2.SkewY,
        t1.ScaleX * t2.SkewX + t1.SkewX * t2.ScaleY,
        t1.SkewY * t2.ScaleX + t1.ScaleY * t2.SkewY,
        t1.SkewY * t2.SkewX + t1.ScaleY * t2.ScaleY,
        t1.ScaleX * t2.TranslateX + t1.SkewX * t2.TranslateY + t1.TranslateX,
        t1.SkewY * t2.TranslateX + t1.ScaleY * t2.TranslateY + t1.TranslateY
    );

    public static (double, double) operator *(Transform2D t, (double, double) p) => (
        t.ScaleX * p.Item1 + t.SkewX * p.Item2 + t.TranslateX,
        t.SkewY * p.Item1 + t.ScaleY * p.Item2 + t.TranslateY
    );

    public static Transform2D operator *(Transform2D t, double f) => new(
        t.ScaleX * f, t.SkewX * f,
        t.SkewY * f, t.ScaleY * f,
        t.TranslateX * f, t.TranslateY * f
    );

    public static bool Invert(Transform2D t, out Transform2D inverse)
    {
        double det = t.Determinant;
        if (det == 0)
        {
            inverse = default;
            return false;
        }

        double invDet = 1.0f / t.Determinant;
        inverse = new()
        {
            ScaleX = t.ScaleY * invDet,
            SkewY = -t.SkewY * invDet,
            SkewX = -t.SkewX * invDet,
            ScaleY = t.ScaleX * invDet,
            TranslateX = (t.SkewX * t.TranslateY - t.TranslateX * t.ScaleY) * invDet,
            TranslateY = (t.TranslateX * t.SkewY - t.ScaleX * t.TranslateY) * invDet,
        };
        return true;
    }

    public double Determinant => ScaleX * ScaleY - SkewX * SkewY;
}