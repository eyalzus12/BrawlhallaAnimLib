namespace BrawlhallaAnimLib.Gfx;

public interface IColorSwap
{
    int ArtType { get; }
    uint OldColor { get; } // u24
    uint NewColor { get; } // u24
}