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

    // TODO: maybe expose those as public?
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

    public CostumeTypesGfx(ICsvRow row)
    {
        List<InternalCustomArtImpl>? baseCustomArts = null;
        List<InternalCustomArtImpl>? swapCustomArts = null;
        InternalCustomArtImpl? headCustomArt = null;
        InternalCustomArtImpl? capeCustomArt = null;

        List<InternalColorSwapImpl>? baseColorSwaps = null;

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
                BoneOverrides[parts[0]] = parts[1];
            }
            else if (key == "UseRightTorso")
            {
                UseRightTorso = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightJaw")
            {
                UseRightJaw = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightEyes")
            {
                UseRightEyes = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightHair")
            {
                UseRightHair = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightMouth")
            {
                UseRightMouth = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightForearm")
            {
                UseRightForearm = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightShoulder1")
            {
                UseRightShoulder1 = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightLeg1")
            {
                UseRightLeg1 = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightShin")
            {
                UseRightShin = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseTrueLeftRightHands")
            {
                UseTrueLeftRightHands = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "HidePaperDollRightPistol")
            {
                HidePaperDollRightPistol = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key.StartsWith("GfxType.CustomArt"))
            {
                baseCustomArts ??= [];
                baseCustomArts.Add(FromCustomArtCell(value, true, ArtTypeEnum.Costume));
            }
            else if (key.StartsWith("SwapCustomArt"))
            {
                swapCustomArts ??= [];
                swapCustomArts.Add(FromCustomArtCell(value, false, ArtTypeEnum.None));
            }
            else if (key == "HeadGfxCustomArt")
            {
                headCustomArt = FromCustomArtCell(value, false, ArtTypeEnum.None);
            }
            else if (key == "DefaultCape")
            {
                capeCustomArt = FromCustomArtCell(value, true, ArtTypeEnum.Costume);
            }
            else if (key.StartsWith("GfxType.ColorSwap"))
            {
                InternalColorSwapImpl colorSwap = FromColorSwapCell(value);
                baseColorSwaps ??= [];
                baseColorSwaps.Add(colorSwap);
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

        if (baseCustomArts is not null)
            CustomArtsInternal.AddRange(baseCustomArts);
        if (swapCustomArts is not null)
            CustomArtsInternal.AddRange(swapCustomArts);
        if (headCustomArt is not null)
            CustomArtsInternal.Add(headCustomArt);
        CustomArtsInternal.Add(capeCustomArt ?? NoCapeCustomArt);

        // TODO: color exceptions

        if (baseColorSwaps is not null)
            ColorSwapsInternal.AddRange(baseColorSwaps);
    }

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

    private static InternalCustomArtImpl FromCustomArtCell(string value, bool grabType, ArtTypeEnum defaultType)
    {
        bool right = grabType && value.StartsWith("RIGHT:");

        ArtTypeEnum type = defaultType;
        if (value.StartsWith("C:")) type = ArtTypeEnum.Costume;
        else if (value.StartsWith("W:")) type = ArtTypeEnum.Weapon;

        string rest = grabType ? value[(value.IndexOf(':') + 1)..] : value;
        string[] parts = rest.Split('/');

        if (parts.Length < 2)
            throw new ArgumentException($"Invalid CustomArt string {rest}");

        string fileName = parts[0];
        string name = parts[1];

        return new()
        {
            Right = right,
            Type = type,
            FileName = fileName,
            Name = name,
        };
    }

    private static InternalColorSwapImpl FromColorSwapCell(string value)
    {
        string[] parts = value.Split('=');
        if (parts.Length != 2)
            throw new ArgumentException($"Invalid color swap string {value}");

        string oldColorString = parts[0];
        if (oldColorString[0] != '0')
            throw new NotImplementedException($"Color swap color must start with 0");
        uint oldColor = ParserUtils.ParseHexString(oldColorString);

        string newColorString = parts[1];
        if (newColorString[0] != '0')
            throw new NotImplementedException($"Color swap color must start with 0");
        uint newColor = ParserUtils.ParseHexString(newColorString);

        return new()
        {
            ArtType = ArtTypeEnum.Costume,
            OldColor = oldColor,
            NewColor = newColor,
        };
    }
}