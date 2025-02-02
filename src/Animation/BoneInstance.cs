using BrawlhallaAnimLib.Anm;

namespace BrawlhallaAnimLib.Animation;

internal sealed class BoneInstance
{
    public required string FilePath { get; init; }
    public required string OgBoneName { get; init; }
    public required string SpriteName { get; init; }
    public required IAnmBone Bone { get; init; }
    public required bool Visible { get; set; }
}