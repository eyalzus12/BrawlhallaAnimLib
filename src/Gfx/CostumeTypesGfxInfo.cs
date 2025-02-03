using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BrawlhallaAnimLib.Bones;

namespace BrawlhallaAnimLib.Gfx;

public sealed class CostumeTypesGfxInfo
{
    internal uint BoneTypeFlags { get; set; } = 0;
    public bool HasAsymmetrySwapFlag(BoneTypeEnum flag) => (BoneTypeFlags & (1u << (int)flag)) != 0;

    internal List<ICustomArt> CustomArtsInternal = [];
    public IEnumerable<ICustomArt> CustomArts => CustomArtsInternal;
    internal List<IColorSwap> ColorSwapsInternal = [];
    public IEnumerable<IColorSwap> ColorSwaps => ColorSwapsInternal;

    // these are set in costumeTypes.csv.
    public bool UseRightTorso { get; internal set; }
    public bool UseRightJaw { get; internal set; }
    public bool UseRightEyes { get; internal set; }
    public bool UseRightMouth { get; internal set; }
    public bool UseRightHair { get; internal set; }
    public bool UseRightForearm { get; internal set; }
    public bool UseRightShoulder1 { get; internal set; }
    public bool UseRightLeg1 { get; internal set; }
    public bool UseRightShin { get; internal set; }
    public bool UseTrueLeftRightHands { get; internal set; }

    internal Dictionary<string, string> BoneOverrides { get; set; } = [];
    public bool TryGetBoneOverride(string bone, [MaybeNullWhen(false)] out string newBone) => BoneOverrides.TryGetValue(bone, out newBone);
}