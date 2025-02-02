namespace BrawlhallaAnimLib.Anm;

public interface IAnmBone
{
    short Id { get; }
    float ScaleX { get; }
    float RotateSkew0 { get; }
    float RotateSkew1 { get; }
    float ScaleY { get; }
    float X { get; }
    float Y { get; }
    double Opacity { get; }
    short Frame { get; }
}