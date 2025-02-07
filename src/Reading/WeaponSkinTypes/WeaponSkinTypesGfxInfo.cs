using System;
using System.Collections.Generic;
using BrawlhallaAnimLib.Gfx;
using BrawlhallaAnimLib.Reading.CostumeTypes;

namespace BrawlhallaAnimLib.Reading.WeaponSkinTypes;

public sealed class WeaponSkinTypesGfxInfo
{
    internal uint AsymmetrySwapFlags { get; set; } = 0;
    internal bool UseRightGauntlet { get; set; }
    internal bool UseRightKatar { get; set; }
    internal bool HideRightPistol2D { get; set; }
    internal List<InternalCustomArtImpl> CustomArtsInternal { get; } = [];
    internal List<InternalColorSwapImpl> ColorSwapsInternal { get; } = [];

    internal bool HasPickupCustomArt { get; set; } = false;
    internal Dictionary<ColorSchemeSwapEnum, uint> SwapDefines { get; } = [];
    internal string? BaseWeapon { get; set; } = null;
    internal ColorSchemeSwapEnum? AttackFxLt_Enum { get; set; } = null;
    internal uint AttackFxLt_Color { get; set; } = 0;
    internal ColorSchemeSwapEnum? AttackFxDk_Enum { get; set; } = null;
    internal uint AttackFxDk_Color { get; set; } = 0;

    public IGfxType ToGfxType(IGfxType gfxType, IColorSchemeType? colorScheme = null, CostumeTypesGfxInfo? costumeType = null)
    {
        // weapon skin types only modify the existing gfx
        InternalGfxImpl gfxResult = new()
        {
            AnimFile = gfxType.AnimFile,
            AnimClass = gfxType.AnimClass,
            AnimScale = gfxType.AnimScale,
            Tint = gfxType.Tint,
            AsymmetrySwapFlags = AsymmetrySwapFlags | gfxType.AsymmetrySwapFlags,
            UseRightTorso = gfxType.UseRightTorso,
            UseRightJaw = gfxType.UseRightJaw,
            UseRightEyes = gfxType.UseRightEyes,
            UseRightMouth = gfxType.UseRightMouth,
            UseRightHair = gfxType.UseRightHair,
            UseRightForearm = gfxType.UseRightForearm,
            UseRightShoulder1 = gfxType.UseRightShoulder1,
            UseRightLeg1 = gfxType.UseRightLeg1,
            UseRightShin = gfxType.UseRightShin,
            UseTrueLeftRightHands = gfxType.UseTrueLeftRightHands,
            HidePaperDollRightPistol = gfxType.HidePaperDollRightPistol,
            CustomArtsInternal = [.. gfxType.CustomArts, .. CustomArtsInternal],
            ColorSwapsInternal = [.. gfxType.ColorSwaps, .. ColorSwapsInternal],
            BoneOverrideDelegate = gfxType.TryGetBoneOverride,
            UseRightGauntlet = UseRightGauntlet,
            UseRightKatar = UseRightKatar,
            HideRightPistol2D = HideRightPistol2D,
            UseTrueLeftRightTorso = gfxType.UseTrueLeftRightTorso,
        };

        ColorSchemeSwapEnum[] swapTypesList = Enum.GetValues<ColorSchemeSwapEnum>();
        if (colorScheme is not null)
        {
            foreach (ColorSchemeSwapEnum swapType in swapTypesList)
            {
                // TODO: color exception

                uint sourceColor = SwapDefines.GetValueOrDefault(swapType, 0u);
                if (sourceColor == 0) continue;
                uint targetColor = colorScheme.GetSwap(swapType);
                if (targetColor == 0) continue;
                InternalColorSwapImpl colorSwap = new()
                {
                    ArtType = ArtTypeEnum.Weapon,
                    OldColor = sourceColor,
                    NewColor = targetColor,
                };
                gfxResult.ColorSwapsInternal.Add(colorSwap);
            }
        }

        InternalColorSwapImpl? getColorSwap(ColorSchemeSwapEnum? swap, uint color, uint sourceColor)
        {
            if (swap is not null)
            {
                ColorSchemeSwapEnum swapType = swap.Value;
                uint targetColor = colorScheme?.GetSwap(swapType) ?? 0;
                if (targetColor == 0)
                {
                    uint definedColor = SwapDefines.GetValueOrDefault(swapType, 0u);
                    targetColor = definedColor;
                }
                return new()
                {
                    ArtType = ArtTypeEnum.Weapon,
                    OldColor = sourceColor,
                    NewColor = targetColor,
                };
            }
            else if (color != 0)
            {
                return new()
                {
                    ArtType = ArtTypeEnum.Weapon,
                    OldColor = sourceColor,
                    NewColor = color,
                };
            }
            return null;
        }

        InternalColorSwapImpl? AttackFxLt = getColorSwap(AttackFxLt_Enum, AttackFxLt_Color, 0x14D3FF);
        if (AttackFxLt is not null) gfxResult.ColorSwapsInternal.Add(AttackFxLt);
        InternalColorSwapImpl? AttackFxDk = getColorSwap(AttackFxDk_Enum, AttackFxDk_Color, 0x004DCC);
        if (AttackFxDk is not null) gfxResult.ColorSwapsInternal.Add(AttackFxDk);

        if (HasPickupCustomArt)
        {
            foreach ((ColorSchemeSwapEnum a, ColorSchemeSwapEnum b) in PickupColorSwapTypes)
            {
                uint source = SwapDefines.GetValueOrDefault(a, 0u);
                uint target = colorScheme?.GetSwap(b) ?? 0;
                InternalColorSwapImpl colorSwap = new()
                {
                    ArtType = ArtTypeEnum.Pickup,
                    OldColor = source,
                    NewColor = target,
                };
                gfxResult.ColorSwapsInternal.Add(colorSwap);
            }
        }

        if (BaseWeapon == "Katar" && costumeType is not null && costumeType.SwapDefines is not null)
        {
            uint getSwap(ColorSchemeSwapEnum swapType)
            {
                if (colorScheme is not null) return colorScheme.GetSwap(swapType);
                return costumeType.SwapDefines.GetValueOrDefault(swapType, 0u);
            }

            foreach ((ColorSchemeSwapEnum swapType, uint source, uint fallbackTarget) in KatarsLightsaberSwapTypes)
            {
                uint target = 0;
                // indirect
                if (costumeType.IndirectSwaps.TryGetValue(swapType, out ColorSchemeSwapEnum newSwap))
                {
                    uint newTarget = getSwap(newSwap);
                    if (newTarget != 0) target = newTarget;
                }
                // direct
                if (target == 0)
                {
                    if (costumeType.DirectSwaps.TryGetValue(swapType, out uint newTarget))
                    {
                        target = newTarget;
                    }
                }
                // fallback
                if (target == 0)
                {
                    target = fallbackTarget;
                }

                if (target != 0)
                {
                    InternalColorSwapImpl colorSwap = new()
                    {
                        ArtType = ArtTypeEnum.Weapon,
                        OldColor = source,
                        NewColor = target,
                    };
                    gfxResult.ColorSwapsInternal.Add(colorSwap);
                }
            }
        }

        return gfxResult;
    }

    private static readonly (ColorSchemeSwapEnum, ColorSchemeSwapEnum)[] PickupColorSwapTypes = [
        (ColorSchemeSwapEnum.SpecialAcc, ColorSchemeSwapEnum.SpecialAcc),
        (ColorSchemeSwapEnum.SpecialDk, ColorSchemeSwapEnum.SpecialDk),
        (ColorSchemeSwapEnum.SpecialLt, ColorSchemeSwapEnum.SpecialLt),
        (ColorSchemeSwapEnum.Special, ColorSchemeSwapEnum.SpecialDk),
        (ColorSchemeSwapEnum.SpecialVL, ColorSchemeSwapEnum.SpecialLt),
    ];

    private static readonly (ColorSchemeSwapEnum swapType, uint source, uint fallbackTarget)[] KatarsLightsaberSwapTypes = [
        (ColorSchemeSwapEnum.HandsLt, 0x54ABEB, 0),
        (ColorSchemeSwapEnum.HandsSkinLt, 0x54ABEB, 0xFFCC99),
        (ColorSchemeSwapEnum.HandsDk, 0xBFFFFC, 0),
        (ColorSchemeSwapEnum.HandsSkinDk, 0xBFFFFC, 0xFF926C),
        (ColorSchemeSwapEnum.HandsSkinLt, 0xFFCC99, 0),
        (ColorSchemeSwapEnum.HandsSkinDk, 0xFFCC99, 0),
    ];
}