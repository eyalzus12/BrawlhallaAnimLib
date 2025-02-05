namespace BrawlhallaAnimLib.Gfx;

public interface ICustomArt
{
    bool Right { get; }
    ArtTypeEnum Type { get; }

    string FileName { get; }
    string Name { get; }
}