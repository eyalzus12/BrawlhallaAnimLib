namespace BrawlhallaAnimLib.Gfx;

public interface ISpriteData
{
    string SetName { get; }
    string BoneName { get; }
    string File { get; }
    double Width { get; }
    double Height { get; }
    double XOffset { get; }
    double YOffset { get; }
}