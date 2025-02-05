using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BrawlhallaAnimLib.Bones;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Csv;

public static class CostumeTypesCsvReader
{
    private static ICustomArt NoCapeCustomArt => new InternalCustomArtImpl()
    {
        FileName = "Gfx_Player.swf",
        Name = "NoCape",
        Type = 2,
    };

    public static CostumeTypesGfxInfo GetGfxTypeInfo(ICsvRow row, IColorSchemeType? colorScheme)
    {
        CostumeTypesGfxInfo gfx = new();

        List<ICustomArt>? baseCustomArts = null;
        List<ICustomArt>? swapCustomArts = null;
        ICustomArt? headCustomArt = null;
        ICustomArt? capeCustomArt = null;

        List<IColorSwap>? baseColorSwaps = null;
        Dictionary<ColorSchemeSwapEnum, uint>? swapDefines = null;
        Dictionary<ColorSchemeSwapEnum, ColorSchemeSwapEnum>? swapTypeFallback = null;
        Dictionary<ColorSchemeSwapEnum, uint>? directSwaps = null;

        foreach ((string key, string value) in row.ColEntries)
        {
            if (value == "") continue;

            if (key == "GfxType.AsymmetrySwapFlags")
            {
                uint asf = value.Split(",").Select((flag) =>
                {
                    if (Enum.TryParse(flag, out BoneTypeEnum result))
                        return 1u << (int)result;
                    return 0u;
                }).Aggregate((a, v) => a | v);

                gfx.BoneTypeFlags = asf;
            }
            else if (key.StartsWith("GfxType.CustomArt"))
            {
                baseCustomArts ??= [];
                baseCustomArts.Add(FromCustomArtCell(value, true, 2));
            }
            else if (key.StartsWith("SwapCustomArt"))
            {
                swapCustomArts ??= [];
                swapCustomArts.Add(FromCustomArtCell(value, false, 0));
            }
            else if (key == "HeadGfxCustomArt")
            {
                headCustomArt = FromCustomArtCell(value, false, 0);
            }
            else if (key == "DefaultCape")
            {
                capeCustomArt = FromCustomArtCell(value, true, 2);
            }
            else if (key.StartsWith("GfxType.ColorSwap"))
            {
                IColorSwap colorSwap = FromColorSwapCell(value);
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
                IColorSwap colorSwap = new InternalColorSwapImpl()
                {
                    ArtType = 2,
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
                IColorSwap colorSwap = new InternalColorSwapImpl()
                {
                    ArtType = 2,
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
            IColorSwap colorSwap = new InternalColorSwapImpl()
            {
                ArtType = 2,
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
            IColorSwap colorSwap = new InternalColorSwapImpl()
            {
                ArtType = 2,
                OldColor = sourceColor,
                NewColor = targetColor,
            };
            gfx.ColorSwapsInternal.Add(colorSwap);
        }

        return gfx;
    }

    private static InternalCustomArtImpl FromCustomArtCell(string value, bool grabType, int defaultType)
    {
        bool right = grabType && value.StartsWith("RIGHT:");
        int type = grabType ? right ? 0 : (value.StartsWith("C:") ? 2 : (value.StartsWith("W:") ? 1 : 0)) : 0;
        if (type == 0) type = defaultType;

        string rest = grabType ? value[(value.IndexOf(':') + 1)..] : value;
        string[] parts = rest.Split('/');
        string fileName = parts[0];
        string name = parts[1];

        return new InternalCustomArtImpl()
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

        return new InternalColorSwapImpl()
        {
            ArtType = 1,
            OldColor = oldColor,
            NewColor = newColor,
        };
    }
}