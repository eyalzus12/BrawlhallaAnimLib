namespace BrawlhallaAnimLib.Gfx;

internal sealed class InternalColorSwapImpl : IColorSwap
{
    public ArtTypeEnum ArtType { get; internal set; }
    public uint OldColor { get; internal set; }
    public uint NewColor { get; internal set; }
}