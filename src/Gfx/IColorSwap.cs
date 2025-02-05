namespace BrawlhallaAnimLib.Gfx;

public interface IColorSwap
{
    ArtTypeEnum ArtType { get; }
    uint OldColor { get; } // u24
    uint NewColor { get; } // u24
}