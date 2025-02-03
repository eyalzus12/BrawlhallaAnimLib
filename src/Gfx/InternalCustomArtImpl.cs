namespace BrawlhallaAnimLib.Gfx;

internal sealed class InternalCustomArtImpl : ICustomArt
{
    internal InternalCustomArtImpl() { }

    public bool Right { get; internal set; } = false;
    public int Type { get; internal set; } = 0;
    public string FileName { get; internal set; } = null!;
    public string Name { get; internal set; } = null!;
}