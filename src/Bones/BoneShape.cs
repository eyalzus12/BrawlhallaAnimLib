using System.Collections.Generic;
using BrawlhallaAnimLib.Math;

namespace BrawlhallaAnimLib.Bones;

public sealed class BoneShape
{
    public required string SwfFilePath { get; init; }
    public required string BoneName { get; init; }
    public required ushort ShapeId { get; init; }

    public required double AnimScale { get; init; }
    public required Transform2D Transform { get; init; }

    // TODO: color transform
    public required uint Tint { get; init; } // u24
    public Dictionary<uint, uint> ColorSwapDict { get; init; } = [];
    public required double Opacity { get; init; }
}