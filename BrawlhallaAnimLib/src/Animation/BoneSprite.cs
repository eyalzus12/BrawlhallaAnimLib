using BrawlhallaAnimLib.Gfx;
using BrawlhallaAnimLib.Math;

namespace BrawlhallaAnimLib.Animation;

public sealed class BoneSprite
{
    public required string SwfFilePath { get; init; }
    public required string SpriteName { get; init; }

    public required long Frame { get; init; }
    public required double AnimScale { get; init; }
    public required Transform2D Transform { get; init; }

    public required uint Tint { get; init; } // u24
    public required IColorSwap[] ColorSwaps { get; init; }
    public required double Opacity { get; init; }
}