using System;
using System.Collections.Generic;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Reading.CostumeTypes;

public sealed class CostumeTypesGfxInfo
{
    internal string CostumeName { get; set; } = null!;
    internal uint AsymmetrySwapFlags { get; set; } = 0;
    internal bool UseRightTorso { get; set; }
    internal bool UseRightJaw { get; set; }
    internal bool UseRightEyes { get; set; }
    internal bool UseRightMouth { get; set; }
    internal bool UseRightHair { get; set; }
    internal bool UseRightForearm { get; set; }
    internal bool UseRightShoulder1 { get; set; }
    internal bool UseRightLeg1 { get; set; }
    internal bool UseRightShin { get; set; }
    internal bool UseTrueLeftRightHands { get; set; }
    internal bool HidePaperDollRightPistol { get; set; }
    internal List<InternalCustomArtImpl> CustomArtsInternal { get; } = [];
    internal List<InternalColorSwapImpl> ColorSwapsInternal { get; } = [];
    internal Dictionary<string, string> BoneOverrides { get; } = [];

    internal Dictionary<ColorSchemeSwapEnum, uint> SwapDefines { get; } = [];
    internal Dictionary<ColorSchemeSwapEnum, ColorSchemeSwapEnum> IndirectSwaps { get; } = [];
    internal Dictionary<ColorSchemeSwapEnum, uint> DirectSwaps { get; } = [];

    public IGfxType ToGfxType(IGfxType gfxType, IColorSchemeType? colorScheme = null, IColorExceptionTypes? colorExceptions = null)
    {
        InternalGfxImpl gfxResult = new()
        {
            AnimFile = gfxType.AnimFile,
            AnimClass = gfxType.AnimClass,
            AnimScale = gfxType.AnimScale,
            Tint = gfxType.Tint,
            AsymmetrySwapFlags = AsymmetrySwapFlags,
            UseRightTorso = UseRightTorso,
            UseRightJaw = UseRightJaw,
            UseRightEyes = UseRightEyes,
            UseRightMouth = UseRightMouth,
            UseRightHair = UseRightHair,
            UseRightForearm = UseRightForearm,
            UseRightShoulder1 = UseRightShoulder1,
            UseRightLeg1 = UseRightLeg1,
            UseRightShin = UseRightShin,
            UseTrueLeftRightHands = UseTrueLeftRightHands,
            HidePaperDollRightPistol = HidePaperDollRightPistol,
            CustomArtsInternal = [.. CustomArtsInternal],
            ColorSwapsInternal = [.. ColorSwapsInternal],
            BoneOverrides = new([.. BoneOverrides]), // clone
        };

        /*
        There's probably some mistake in this code
        Gotta revisit this later
        */

        ColorSchemeSwapEnum[] swapTypesList = Enum.GetValues<ColorSchemeSwapEnum>();
        if (colorScheme is not null)
        {
            IColorExceptionType? colorException = null;
            colorExceptions?.TryGetColorException(
                CostumeName, colorScheme.Name, ColorExceptionMode.Costume,
                out colorException
            );

            // color scheme
            foreach (ColorSchemeSwapEnum swapType in swapTypesList)
            {
                ColorSchemeSwapEnum targetSwapType = swapType;
                if (colorException?.TryGetSwapRedirect(swapType, out ColorSchemeSwapEnum newSwapType) ?? false)
                {
                    targetSwapType = newSwapType;
                }

                uint sourceColor = SwapDefines.GetValueOrDefault(swapType, 0u);
                if (sourceColor == 0) continue;
                uint targetColor = colorScheme.GetSwap(targetSwapType);
                if (targetColor == 0) continue;
                InternalColorSwapImpl colorSwap = new()
                {
                    ArtType = ArtTypeEnum.Costume,
                    OldColor = sourceColor,
                    NewColor = targetColor,
                };
                gfxResult.ColorSwapsInternal.Add(colorSwap);
            }
            // get swap from scheme
            foreach (ColorSchemeSwapEnum swapType in swapTypesList)
            {
                // if has swap for this type, ignore
                uint schemeTargetColor = colorScheme.GetSwap(swapType);
                if (schemeTargetColor != 0) continue;
                // get source for fallback
                uint sourceColor = SwapDefines.GetValueOrDefault(swapType, 0u);
                if (sourceColor == 0) continue;
                // get indirect swap type
                if (!IndirectSwaps.TryGetValue(swapType, out ColorSchemeSwapEnum targetSwapType))
                    continue;
                // get target from scheme
                uint targetColor = colorScheme.GetSwap(targetSwapType);
                if (targetColor == 0) continue;
                InternalColorSwapImpl colorSwap = new()
                {
                    ArtType = ArtTypeEnum.Costume,
                    OldColor = sourceColor,
                    NewColor = targetColor,
                };
                gfxResult.ColorSwapsInternal.Add(colorSwap);
            }
        }
        // defines as fallback
        foreach (ColorSchemeSwapEnum swapType in swapTypesList)
        {
            // if has swap for this type, ignore
            uint schemeTargetColor = colorScheme?.GetSwap(swapType) ?? 0;
            if (schemeTargetColor != 0) continue;
            // get source for fallback
            uint sourceColor = SwapDefines.GetValueOrDefault(swapType, 0u);
            if (sourceColor == 0) continue;
            // get indirect swap type
            if (!IndirectSwaps.TryGetValue(swapType, out ColorSchemeSwapEnum targetSwapType))
                continue;
            // get target from defines
            uint targetColor = SwapDefines.GetValueOrDefault(targetSwapType, 0u);
            if (targetColor == 0) continue;
            InternalColorSwapImpl colorSwap = new()
            {
                ArtType = ArtTypeEnum.Costume,
                OldColor = sourceColor,
                NewColor = targetColor,
            };
            gfxResult.ColorSwapsInternal.Add(colorSwap);
        }
        // direct swaps
        foreach (ColorSchemeSwapEnum swapType in swapTypesList)
        {
            // if has swap for this type, ignore
            uint schemeTargetColor = colorScheme?.GetSwap(swapType) ?? 0;
            if (schemeTargetColor != 0) continue;

            // get source for direct swap
            uint sourceColor = SwapDefines.GetValueOrDefault(swapType, 0u);
            if (sourceColor == 0) continue;
            uint targetColor = DirectSwaps.GetValueOrDefault(swapType, 0u);
            if (targetColor == 0) continue;
            InternalColorSwapImpl colorSwap = new()
            {
                ArtType = ArtTypeEnum.Costume,
                OldColor = sourceColor,
                NewColor = targetColor,
            };
            gfxResult.ColorSwapsInternal.Add(colorSwap);
        }

        return gfxResult;
    }
}