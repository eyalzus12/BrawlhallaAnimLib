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

    public static CostumeTypesGfxInfo GetGfxTypeInfo(ICsvRow row)
    {
        CostumeTypesGfxInfo gfx = new();

        List<ICustomArt>? baseCustomArts = null;
        List<ICustomArt>? swapCustomArts = null;
        ICustomArt? headCustomArt = null;
        ICustomArt? capeCustomArt = null;
        foreach ((string key, string value) in row.ColEntries)
        {
            if (value == "") continue;

            if (key == "GfxType.AsymmetrySwapFlags")
            {
                uint asf = value.Split(",").Select((flag) =>
                {
                    if (Enum.TryParse<BoneTypeEnum>(flag, out BoneTypeEnum result))
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
                gfx.ColorSwapsInternal.Add(FromColorSwapCell(value));
            }
        }

        if (baseCustomArts is not null)
            gfx.CustomArtsInternal.AddRange(baseCustomArts);
        if (swapCustomArts is not null)
            gfx.CustomArtsInternal.AddRange(swapCustomArts);
        if (headCustomArt is not null)
            gfx.CustomArtsInternal.Add(headCustomArt);
        gfx.CustomArtsInternal.Add(capeCustomArt ?? NoCapeCustomArt);

        return gfx;
    }

    private static ICustomArt FromCustomArtCell(string value, bool grabType, int defaultType)
    {
        bool right = grabType ? value.StartsWith("RIGHT:") : false;
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

    private static IColorSwap FromColorSwapCell(string value)
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