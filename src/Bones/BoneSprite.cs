using BrawlhallaAnimLib.Gfx;
using BrawlhallaAnimLib.Math;

namespace BrawlhallaAnimLib.Bones;

public abstract class BoneSprite
{
    public required string SwfFilePath { get; init; }

    public required long Frame { get; init; }
    public required double AnimScale { get; init; }
    public required Transform2D Transform { get; init; }

    public required uint Tint { get; init; } // u24
    public required IColorSwap[] ColorSwaps { get; init; }
    public required double Opacity { get; init; }
}

public sealed class BoneSpriteWithName : BoneSprite
{
    public required string SpriteName { get; init; }
}

public sealed class BoneSpriteWithId : BoneSprite
{
    public required ushort SpriteId { get; init; }
}