using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BrawlhallaAnimLib.Bones;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Reading;

public static class CostumeTypesCsvReader
{
    private static InternalCustomArtImpl NoCapeCustomArt => new()
    {
        FileName = "Gfx_Player.swf",
        Name = "NoCape",
        Type = ArtTypeEnum.Costume,
    };

    public static CostumeTypesGfxInfo GetGfxTypeInfo(ICsvRow row, IColorSchemeType? colorScheme)
    {
        CostumeTypesGfxInfo gfx = new();

        List<InternalCustomArtImpl>? baseCustomArts = null;
        List<InternalCustomArtImpl>? swapCustomArts = null;
        InternalCustomArtImpl? headCustomArt = null;
        InternalCustomArtImpl? capeCustomArt = null;

        List<InternalColorSwapImpl>? baseColorSwaps = null;
        Dictionary<ColorSchemeSwapEnum, uint>? swapDefines = null;
        Dictionary<ColorSchemeSwapEnum, ColorSchemeSwapEnum>? swapTypeFallback = null;
        Dictionary<ColorSchemeSwapEnum, uint>? directSwaps = null;

        foreach ((string key, string value) in row.ColEntries)
        {
            if (value == "") continue;

            if (key == "GfxType.AsymmetrySwapFlags")
            {
                uint asf = value.Split(",").Select(static (flag) =>
                {
                    if (Enum.TryParse(flag, out BoneTypeEnum result))
                        return 1u << (int)result;
                    return 0u;
                }).Aggregate((a, v) => a | v);

                gfx.BoneTypeFlags = asf;
            }
            else if (key == "BoneOverride")
            {
                string[] parts = value.Split(',');
                gfx.BoneOverrides[parts[0]] = parts[1];
            }
            else if (key == "UseRightTorso")
            {
                gfx.UseRightTorso = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightJaw")
            {
                gfx.UseRightJaw = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightEyes")
            {
                gfx.UseRightEyes = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightHair")
            {
                gfx.UseRightHair = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightMouth")
            {
                gfx.UseRightMouth = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightForearm")
            {
                gfx.UseRightForearm = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightShoulder1")
            {
                gfx.UseRightShoulder1 = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightLeg1")
            {
                gfx.UseRightLeg1 = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightShin")
            {
                gfx.UseRightShin = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseTrueLeftRightHands")
            {
                gfx.UseTrueLeftRightHands = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "HidePaperDollRightPistol")
            {
                gfx.HidePaperDollRightPistol = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
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
                swapDefines ??= [];
                swapDefines[swapType] = uint.Parse(value, CultureInfo.InvariantCulture);
            }
            else if (key.EndsWith("_Swap"))
            {
                string swap = key[..^"_Define".Length];
                if (!Enum.TryParse(swap, true, out ColorSchemeSwapEnum swapType))
                    throw new ArgumentException($"Invalid swap {swap}");

                if (value.StartsWith("0x"))
                {
                    uint direct = Convert.ToUInt32(value, 16);
                    directSwaps ??= [];
                    directSwaps[swapType] = direct;
                }
                else
                {
                    if (!Enum.TryParse(value, true, out ColorSchemeSwapEnum target))
                        throw new ArgumentException($"Invalid swap {value}");
                    swapTypeFallback ??= [];
                    swapTypeFallback[swapType] = target;
                }
            }
        }

        if (baseCustomArts is not null)
            gfx.CustomArtsInternal.AddRange(baseCustomArts);
        if (swapCustomArts is not null)
            gfx.CustomArtsInternal.AddRange(swapCustomArts);
        if (headCustomArt is not null)
            gfx.CustomArtsInternal.Add(headCustomArt);
        gfx.CustomArtsInternal.Add(capeCustomArt ?? NoCapeCustomArt);

        // TODO: color exceptions

        if (baseColorSwaps is not null)
            gfx.ColorSwapsInternal.AddRange(baseColorSwaps);
        ColorSchemeSwapEnum[] swapTypesList = Enum.GetValues<ColorSchemeSwapEnum>();
        if (colorScheme is not null)
        {
            // color scheme
            foreach (ColorSchemeSwapEnum swapType in swapTypesList)
            {
                uint sourceColor = swapDefines?.GetValueOrDefault(swapType, 0u) ?? 0;
                if (sourceColor == 0) continue;
                uint targetColor = colorScheme.GetSwap(swapType);
                if (targetColor == 0) continue;
                InternalColorSwapImpl colorSwap = new()
                {
                    ArtType = ArtTypeEnum.Costume,
                    OldColor = sourceColor,
                    NewColor = targetColor,
                };
                gfx.ColorSwapsInternal.Add(colorSwap);
            }
            // fallback from scheme
            foreach (ColorSchemeSwapEnum swapType in swapTypesList)
            {
                // if has swap for this type, ignore
                uint schemeTargetColor = colorScheme.GetSwap(swapType);
                if (schemeTargetColor != 0) continue;
                // get source for fallback
                uint sourceColor = swapDefines?.GetValueOrDefault(swapType, 0u) ?? 0;
                if (sourceColor == 0) continue;
                // get fallback swap type
                if (swapTypeFallback is null || !swapTypeFallback.TryGetValue(swapType, out ColorSchemeSwapEnum targetSwapType))
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
                gfx.ColorSwapsInternal.Add(colorSwap);
            }
        }
        // defines as fallback
        foreach (ColorSchemeSwapEnum swapType in swapTypesList)
        {
            // if has swap for this type, ignore
            uint schemeTargetColor = colorScheme?.GetSwap(swapType) ?? 0;
            if (schemeTargetColor != 0) continue;
            // get source for fallback
            uint sourceColor = swapDefines?.GetValueOrDefault(swapType, 0u) ?? 0;
            if (sourceColor == 0) continue;
            // get fallback swap type
            if (swapTypeFallback is null || !swapTypeFallback.TryGetValue(swapType, out ColorSchemeSwapEnum targetSwapType))
                continue;
            // get target from defines
            uint targetColor = swapDefines?.GetValueOrDefault(targetSwapType, 0u) ?? 0;
            if (targetColor == 0) continue;
            InternalColorSwapImpl colorSwap = new()
            {
                ArtType = ArtTypeEnum.Costume,
                OldColor = sourceColor,
                NewColor = targetColor,
            };
            gfx.ColorSwapsInternal.Add(colorSwap);
        }
        // direct swaps
        foreach (ColorSchemeSwapEnum swapType in swapTypesList)
        {
            // if has swap for this type, ignore
            uint schemeTargetColor = colorScheme?.GetSwap(swapType) ?? 0;
            if (schemeTargetColor != 0) continue;

            // get source for fallback
            uint sourceColor = swapDefines?.GetValueOrDefault(swapType, 0u) ?? 0;
            if (sourceColor == 0) continue;
            uint targetColor = directSwaps?.GetValueOrDefault(swapType, 0u) ?? 0;
            if (targetColor == 0) continue;
            InternalColorSwapImpl colorSwap = new()
            {
                ArtType = ArtTypeEnum.Costume,
                OldColor = sourceColor,
                NewColor = targetColor,
            };
            gfx.ColorSwapsInternal.Add(colorSwap);
        }

        return gfx;
    }

    private static InternalCustomArtImpl FromCustomArtCell(string value, bool grabType, ArtTypeEnum defaultType)
    {
        bool right = grabType && value.StartsWith("RIGHT:");

        ArtTypeEnum type = defaultType;
        if (value.StartsWith("C:")) type = ArtTypeEnum.Costume;
        else if (value.StartsWith("W:")) type = ArtTypeEnum.Weapon;

        string rest = grabType ? value[(value.IndexOf(':') + 1)..] : value;
        string[] parts = rest.Split('/');
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
        uint oldColor = uint.Parse(oldColorString, CultureInfo.InvariantCulture);

        string newColorString = parts[1];
        if (newColorString[0] != '0')
            throw new NotImplementedException($"Color swap color must start with 0");
        uint newColor = uint.Parse(newColorString, CultureInfo.InvariantCulture);

        return new()
        {
            ArtType = ArtTypeEnum.Costume,
            OldColor = oldColor,
            NewColor = newColor,
        };
    }
}