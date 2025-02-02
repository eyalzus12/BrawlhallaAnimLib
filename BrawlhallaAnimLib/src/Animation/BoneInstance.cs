using WallyAnmSpinzor;

namespace BrawlhallaAnimLib.Animation;

internal sealed class BoneInstance
{
    public required string FilePath { get; init; }
    public required string OgBoneName { get; init; }
    public required string SpriteName { get; init; }
    public required AnmBone Bone { get; init; }
    public required bool Visible { get; set; }
}