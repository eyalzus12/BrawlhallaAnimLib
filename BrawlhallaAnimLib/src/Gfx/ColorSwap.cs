namespace BrawlhallaAnimLib.Gfx;

public interface IColorSwap
{
    int ArtType { get; set; }
    uint OldColor { get; set; } // u24
    uint NewColor { get; set; } // u24
}