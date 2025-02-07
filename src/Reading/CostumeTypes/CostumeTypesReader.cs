using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BrawlhallaAnimLib.Bones;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Reading.CostumeTypes;

public static class CostumeTypesCsvReader
{
    private static InternalCustomArtImpl NoCapeCustomArt => new()
    {
        FileName = "Gfx_Player.swf",
        Name = "NoCape",
        Type = ArtTypeEnum.Costume,
    };

    public static CostumeTypesGfxInfo GetGfxTypeInfo(ICsvRow row)
    {
        CostumeTypesGfxInfo info = new();

        List<InternalCustomArtImpl>? baseCustomArts = null;
        List<InternalCustomArtImpl>? swapCustomArts = null;
        InternalCustomArtImpl? headCustomArt = null;
        InternalCustomArtImpl? capeCustomArt = null;

        List<InternalColorSwapImpl>? baseColorSwaps = null;

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

                info.AsymmetrySwapFlags = asf;
            }
            else if (key == "BoneOverride")
            {
                string[] parts = value.Split(',');
                info.BoneOverrides[parts[0]] = parts[1];
            }
            else if (key == "UseRightTorso")
            {
                info.UseRightTorso = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightJaw")
            {
                info.UseRightJaw = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightEyes")
            {
                info.UseRightEyes = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightHair")
            {
                info.UseRightHair = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightMouth")
            {
                info.UseRightMouth = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightForearm")
            {
                info.UseRightForearm = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightShoulder1")
            {
                info.UseRightShoulder1 = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightLeg1")
            {
                info.UseRightLeg1 = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseRightShin")
            {
                info.UseRightShin = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "UseTrueLeftRightHands")
            {
                info.UseTrueLeftRightHands = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
            }
            else if (key == "HidePaperDollRightPistol")
            {
                info.HidePaperDollRightPistol = value.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase);
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
                info.SwapDefines[swapType] = uint.Parse(value, CultureInfo.InvariantCulture);
            }
            else if (key.EndsWith("_Swap"))
            {
                string swap = key[..^"_Define".Length];
                if (!Enum.TryParse(swap, true, out ColorSchemeSwapEnum swapType))
                    throw new ArgumentException($"Invalid swap {swap}");

                if (value.StartsWith("0x"))
                {
                    uint direct = Convert.ToUInt32(value, 16);
                    info.DirectSwaps[swapType] = direct;
                }
                else
                {
                    if (!Enum.TryParse(value, true, out ColorSchemeSwapEnum target))
                        throw new ArgumentException($"Invalid swap {value}");
                    info.IndirectSwaps[swapType] = target;
                }
            }
        }

        if (baseCustomArts is not null)
            info.CustomArtsInternal.AddRange(baseCustomArts);
        if (swapCustomArts is not null)
            info.CustomArtsInternal.AddRange(swapCustomArts);
        if (headCustomArt is not null)
            info.CustomArtsInternal.Add(headCustomArt);
        info.CustomArtsInternal.Add(capeCustomArt ?? NoCapeCustomArt);

        // TODO: color exceptions

        if (baseColorSwaps is not null)
            info.ColorSwapsInternal.AddRange(baseColorSwaps);

        return info;
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