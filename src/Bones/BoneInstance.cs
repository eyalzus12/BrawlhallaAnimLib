using BrawlhallaAnimLib.Anm;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Bones;

internal abstract class BoneInstance
{
    public required string OgBoneName { get; init; }
    public required IAnmBone Bone { get; init; }
    public required bool Visible { get; set; }
}

internal sealed class SwfBoneInstance : BoneInstance
{
    public required string SwfFilePath { get; init; }
    public required string SpriteName { get; init; }
}

internal sealed class BitmapBoneInstance : BoneInstance
{
    public required ISpriteData SpriteData { get; init; }
}