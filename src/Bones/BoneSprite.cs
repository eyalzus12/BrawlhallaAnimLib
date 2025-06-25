using System.Collections.Generic;
using BrawlhallaAnimLib.Gfx;
using BrawlhallaAnimLib.Math;

namespace BrawlhallaAnimLib.Bones;

public abstract class BoneSprite
{
    public required Transform2D Transform { get; init; }

    public required uint Tint { get; init; } // u24
    public required double Opacity { get; init; }
}

public sealed class BitmapBoneSprite : BoneSprite
{
    public required ISpriteData SpriteData { get; init; }
}

public abstract class SwfBoneSprite : BoneSprite
{
    public required string SwfFilePath { get; init; }
    public required long Frame { get; init; }
    public required double AnimScale { get; init; }
    public Dictionary<uint, uint> ColorSwapDict { get; init; } = [];
}

public sealed class SwfBoneSpriteWithName : SwfBoneSprite
{
    public required string SpriteName { get; init; }
}

public sealed class SwfBoneSpriteWithId : SwfBoneSprite
{
    public required ushort SpriteId { get; init; }
}