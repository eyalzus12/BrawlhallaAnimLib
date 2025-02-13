using BrawlhallaAnimLib.Math;

namespace BrawlhallaAnimLib.Bones;

public sealed class BoneShape
{
    // color swap dict, AnimScale, etc are inherited from sprite

    public required ushort ShapeId { get; init; }
    public required Transform2D Transform { get; init; }
    // TODO: color transform
    public required uint Tint { get; init; } // u24
}