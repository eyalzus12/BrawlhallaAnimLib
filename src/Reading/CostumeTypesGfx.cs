using System;
using System.Collections.Generic;
using System.Linq;
using BrawlhallaAnimLib.Bones;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Reading.CostumeTypes;

public sealed class CostumeTypesGfx
{
    private static InternalCustomArtImpl NoCapeCustomArt => new()
    {
        FileName = "Gfx_Player.swf",
        Name = "NoCape",
        Type = ArtTypeEnum.Costume,
    };

    internal string CostumeName { get; }
    internal uint AsymmetrySwapFlags { get; } = 0;
    internal bool UseRightTorso { get; } = false;
    internal bool UseRightJaw { get; } = false;
    internal bool UseRightEyes { get; } = false;
    internal bool UseRightMouth { get; } = false;
    internal bool UseRightHair { get; } = false;
    internal bool UseRightForearm { get; } = false;
    internal bool UseRightShoulder1 { get; } = false;
    internal bool UseRightLeg1 { get; } = false;
    internal bool UseRightShin { get; } = false;
    internal bool UseTrueLeftRightHands { get; } = false;
    internal bool HidePaperDollRightPistol { get; } = false;
    internal List<InternalCustomArtImpl> BaseCustomArts { get; } = [];
    internal List<InternalCustomArtImpl> SwapCustomArts { get; } = [];
    internal InternalCustomArtImpl? HeadCustomArt { get; } = null;
    internal InternalCustomArtImpl? CapeCustomArt { get; } = null;
    internal List<InternalColorSwapImpl> BaseColorSwaps { get; } = [];
    internal Dictionary<string, string> BoneOverride { get; } = [];

    internal Dictionary<ColorSchemeSwapEnum, uint> SwapDefines { get; } = [];
    internal Dictionary<ColorSchemeSwapEnum, ColorSchemeSwapEnum> IndirectSwaps { get; } = [];
    internal Dictionary<ColorSchemeSwapEnum, uint> DirectSwaps { get; } = [];

    public CostumeTypesGfx(ICsvRow row)
    {
        foreach ((string key, string value) in row.ColEntries)
        {
            if (value == "") continue;

            if (key == "CostumeName")
            {
                CostumeName = value;
            }
            else if (key == "GfxType.AsymmetrySwapFlags")
            {
                uint asf = value.Split(",").Select(static (flag) =>
                {
                    if (Enum.TryParse(flag, out BoneTypeEnum result))
                        return 1u << (int)result;
                    return 0u;
                }).Aggregate((a, v) => a | v);

                AsymmetrySwapFlags = asf;
            }
            else if (key.StartsWith("BoneOverride"))
            {
                string[] parts = value.Split(',');
                BoneOverride[parts[0]] = parts[1];
            }
            else if (key == "UseRightTorso")
            {
                UseRightTorso = ParserUtils.ParseBool(value);
            }
            else if (key == "UseRightJaw")
            {
                UseRightJaw = ParserUtils.ParseBool(value);
            }
            else if (key == "UseRightEyes")
            {
                UseRightEyes = ParserUtils.ParseBool(value);
            }
            else if (key == "UseRightHair")
            {
                UseRightHair = ParserUtils.ParseBool(value);
            }
            else if (key == "UseRightMouth")
            {
                UseRightMouth = ParserUtils.ParseBool(value);
            }
            else if (key == "UseRightForearm")
            {
                UseRightForearm = ParserUtils.ParseBool(value);
            }
            else if (key == "UseRightShoulder1")
            {
                UseRightShoulder1 = ParserUtils.ParseBool(value);
            }
            else if (key == "UseRightLeg1")
            {
                UseRightLeg1 = ParserUtils.ParseBool(value);
            }
            else if (key == "UseRightShin")
            {
                UseRightShin = ParserUtils.ParseBool(value);
            }
            else if (key == "UseTrueLeftRightHands")
            {
                UseTrueLeftRightHands = ParserUtils.ParseBool(value);
            }
            else if (key == "HidePaperDollRightPistol")
            {
                HidePaperDollRightPistol = ParserUtils.ParseBool(value);
            }
            else if (key.StartsWith("GfxType.CustomArt"))
            {
                BaseCustomArts.Add(ParserUtils.ParseCustomArt(value, true, ArtTypeEnum.Costume));
            }
            else if (key.StartsWith("SwapCustomArt"))
            {
                SwapCustomArts.Add(ParserUtils.ParseCustomArt(value, false, ArtTypeEnum.None));
            }
            else if (key == "HeadGfxCustomArt")
            {
                HeadCustomArt = ParserUtils.ParseCustomArt(value, false, ArtTypeEnum.None);
            }
            else if (key == "DefaultCape")
            {
                CapeCustomArt = ParserUtils.ParseCustomArt(value, true, ArtTypeEnum.Costume);
            }
            else if (key.StartsWith("GfxType.ColorSwap"))
            {
                InternalColorSwapImpl colorSwap = ParserUtils.ParseColorSwap(value, ArtTypeEnum.Costume);
                BaseColorSwaps.Add(colorSwap);
            }
            else if (key.EndsWith("_Define"))
            {
                string swap = key[..^"_Define".Length];
                if (!Enum.TryParse(swap, true, out ColorSchemeSwapEnum swapType))
                    throw new ArgumentException($"Invalid swap {swap}");
                SwapDefines[swapType] = ParserUtils.ParseHexString(value);
            }
            else if (key.EndsWith("_Swap"))
            {
                string swap = key[..^"_Swap".Length];
                if (!Enum.TryParse(swap, true, out ColorSchemeSwapEnum swapType))
                    throw new ArgumentException($"Invalid swap {swap}");

                if (value.StartsWith("0x"))
                {
                    uint direct = ParserUtils.ParseHexString(value);
                    DirectSwaps[swapType] = direct;
                }
                else
                {
                    if (!Enum.TryParse(value, true, out ColorSchemeSwapEnum target))
                        throw new ArgumentException($"Invalid swap {value}");
                    IndirectSwaps[swapType] = target;
                }
            }
        }

        if (CostumeName is null) throw new ArgumentException("Missing CostumeName");
    }

    public IGfxType ToGfxType(IGfxType gfxType, IColorSchemeType? colorScheme = null, IColorExceptionTypes? colorExceptions = null, bool headSwap = false, bool noSwapArt = false)
    {
        InternalGfxImpl gfxResult = new(gfxType)
        {
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
            // this is correct - the custom arts are prepended
            CustomArtsInternal = [.. BaseCustomArts, .. gfxType.CustomArts],
            // TODO: it seems like the game doesn't actually copy this over??
            BoneOverride = new(BoneOverride), // clone
        };
        gfxResult.AsymmetrySwapFlags |= AsymmetrySwapFlags;
        gfxResult.ColorSwapsInternal.AddRange(BaseColorSwaps);

        if (!noSwapArt)
        {
            gfxResult.CustomArtsInternal.AddRange(SwapCustomArts);
        }

        if (headSwap && HeadCustomArt is not null)
        {
            gfxResult.CustomArtsInternal.Add(HeadCustomArt);
        }

        gfxResult.CustomArtsInternal.Add(CapeCustomArt ?? NoCapeCustomArt);

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
                // source is define
                uint sourceColor = SwapDefines.GetValueOrDefault(swapType, 0u);
                if (sourceColor == 0) continue;
                // color exception
                ColorSchemeSwapEnum targetSwapType = swapType;
                if (colorException?.TryGetSwapRedirect(swapType, out ColorSchemeSwapEnum newSwapType) ?? false)
                {
                    if (colorScheme.GetSwap(newSwapType) != 0)
                        targetSwapType = newSwapType;
                }
                // target from scheme
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