using System.Collections.Generic;
using BrawlhallaAnimLib.Bones;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Reading;

public sealed class WeaponSkinTypesGfxInfo
{
    internal uint BoneTypeFlags { get; set; } = 0;
    public bool HasAsymmetrySwapFlag(BoneTypeEnum flag) => (BoneTypeFlags & (1u << (int)flag)) != 0;

    internal List<InternalCustomArtImpl> CustomArtsInternal = [];
    public IEnumerable<ICustomArt> CustomArts => CustomArtsInternal;
    internal List<InternalColorSwapImpl> ColorSwapsInternal = [];
    public IEnumerable<IColorSwap> ColorSwaps => ColorSwapsInternal;

    public bool UseRightGauntlet { get; internal set; }
    public bool UseRightKatar { get; internal set; }
    public bool HideRightPistol2D { get; internal set; }
}