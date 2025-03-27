using System.Diagnostics.CodeAnalysis;

namespace BrawlhallaAnimLib.Gfx;

internal sealed class InternalCustomArtImpl : ICustomArt
{
    internal InternalCustomArtImpl() { }

    [SetsRequiredMembers]
    internal InternalCustomArtImpl(ICustomArt customArt)
    {
        Right = customArt.Right;
        Type = customArt.Type;
        FileName = customArt.FileName;
        Name = customArt.Name;
    }

    public bool Right { get; internal set; } = false;
    public ArtTypeEnum Type { get; internal set; } = 0;
    public required string FileName { get; internal init; }
    public required string Name { get; internal init; }
}