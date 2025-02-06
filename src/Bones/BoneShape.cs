using System.Collections.Generic;
using BrawlhallaAnimLib.Gfx;
using BrawlhallaAnimLib.Math;

namespace BrawlhallaAnimLib.Bones;

public sealed class BoneShape
{
    public required string SwfFilePath { get; init; }
    public required ushort ShapeId { get; init; }

    public required double AnimScale { get; init; }
    public required Transform2D Transform { get; init; }

    // TODO: color transform
    public required uint Tint { get; init; } // u24
    internal Dictionary<(ArtTypeEnum, uint), uint> ColorSwapDict { get; init; } = [];
    public required double Opacity { get; init; }

    public bool TryGetSwappedColor(uint color, ArtTypeEnum artType, out uint newColor)
    {
        if (ColorSwapDict.TryGetValue((artType, color), out uint newColor1) && newColor1 != 0)
        {
            newColor = newColor1;
            return true;
        }
        else if (ColorSwapDict.TryGetValue((ArtTypeEnum.None, color), out uint newColor2) && newColor2 != 0)
        {
            newColor = newColor2;
            return true;
        }
        newColor = 0;
        return false;
    }
}