using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BrawlhallaAnimLib.Gfx;

internal sealed class InternalGfxImpl : IGfxType
{
    public required string AnimFile { get; internal set; }
    public required string AnimClass { get; internal set; }

    public double AnimScale { get; internal set; } = 1;

    public uint Tint { get; internal set; } = 0;

    internal List<ICustomArt> CustomArtsInternal { get; init; } = [];
    public IEnumerable<ICustomArt> CustomArts => CustomArtsInternal;
    internal List<IColorSwap> ColorSwapsInternal { get; init; } = [];
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

    public uint AsymmetrySwapFlags { get; internal set; } = 0;

    internal Dictionary<string, string> BoneOverride { get; init; } = [];
    IReadOnlyDictionary<string, string> IGfxType.BoneOverride => BoneOverride;

    internal InternalGfxImpl() { }

    [SetsRequiredMembers]
    internal InternalGfxImpl(IGfxType gfx)
    {
        AnimFile = gfx.AnimFile;
        AnimClass = gfx.AnimClass;
        AnimScale = gfx.AnimScale;
        Tint = gfx.Tint;
        CustomArtsInternal = [.. gfx.CustomArts];
        ColorSwapsInternal = [.. gfx.ColorSwaps];
        UseRightTorso = gfx.UseRightTorso;
        UseRightJaw = gfx.UseRightJaw;
        UseRightEyes = gfx.UseRightEyes;
        UseRightMouth = gfx.UseRightMouth;
        UseRightHair = gfx.UseRightHair;
        UseRightForearm = gfx.UseRightForearm;
        UseRightShoulder1 = gfx.UseRightShoulder1;
        UseRightLeg1 = gfx.UseRightLeg1;
        UseRightShin = gfx.UseRightShin;
        UseTrueLeftRightHands = gfx.UseTrueLeftRightHands;
        HidePaperDollRightPistol = gfx.HidePaperDollRightPistol;
        UseRightGauntlet = gfx.UseRightGauntlet;
        UseRightKatar = gfx.UseRightKatar;
        HideRightPistol2D = gfx.HideRightPistol2D;
        UseTrueLeftRightTorso = gfx.UseTrueLeftRightTorso;
        AsymmetrySwapFlags = gfx.AsymmetrySwapFlags;
        BoneOverride = new(gfx.BoneOverride);
    }
}