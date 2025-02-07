using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BrawlhallaAnimLib.Bones;

namespace BrawlhallaAnimLib.Gfx;

internal delegate bool TryGetBoneOverride(string bone, [MaybeNullWhen(false)] out string newBone);

internal sealed class InternalGfxImpl : IGfxType
{
    public string AnimFile { get; internal set; } = null!;
    public string AnimClass { get; internal set; } = null!;

    public double AnimScale { get; internal set; } = 1;

    public uint Tint { get; internal set; } = 0;

    internal List<InternalCustomArtImpl> CustomArtsInternal { get; init; } = [];
    public IEnumerable<ICustomArt> CustomArts => CustomArtsInternal;
    internal List<InternalColorSwapImpl> ColorSwapsInternal { get; init; } = [];
    public IEnumerable<IColorSwap> ColorSwaps => ColorSwapsInternal;

    public bool UseRightTorso { get; internal set; } = false;
    public bool UseRightJaw { get; internal set; } = false;
    public bool UseRightEyes { get; internal set; } = false;
    public bool UseRightMouth { get; internal set; } = false;
    public bool UseRightHair { get; internal set; } = false;
    public bool UseRightForearm { get; internal set; } = false;
    public bool UseRightShoulder1 { get; internal set; } = false;
    public bool UseRightLeg1 { get; internal set; } = false;
    public bool UseRightShin { get; internal set; } = false;
    public bool UseTrueLeftRightHands { get; internal set; } = false;
    public bool HidePaperDollRightPistol { get; internal set; } = false;
    public bool UseRightGauntlet { get; internal set; } = false;
    public bool UseRightKatar { get; internal set; } = false;
    public bool HideRightPistol2D { get; internal set; } = false;
    public bool UseTrueLeftRightTorso { get; internal set; } = false;

    internal uint AsymmetrySwapFlags { get; set; } = 0;
    public bool HasAsymmetrySwapFlag(BoneTypeEnum flag) => (AsymmetrySwapFlags & (1u << (int)flag)) != 0;

    internal TryGetBoneOverride? BoneOverrideDelegate { get; init; } = null;
    internal Dictionary<string, string> BoneOverrides { get; init; } = [];
    public bool TryGetBoneOverride(string bone, [MaybeNullWhen(false)] out string newBone)
    {
        if (BoneOverrides.TryGetValue(bone, out newBone)) return true;
        if (BoneOverrideDelegate is not null && BoneOverrideDelegate(bone, out newBone)) return true;
        newBone = null;
        return false;
    }
}