using SwfLib.Data;

namespace BrawlhallaAnimLib.Math;

internal static class MathUtils
{
    public static long SafeMod(long x, long m)
    {
        x %= m;
        if (x < 0) x += m;
        return x;
    }

    public static Transform2D SwfMatrixToTransform(SwfMatrix m) => new(m.ScaleX, m.RotateSkew1, m.RotateSkew0, m.ScaleY, m.TranslateX / 20.0, m.TranslateY / 20.0);
}