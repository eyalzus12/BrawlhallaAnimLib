using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using BrawlhallaAnimLib.Bones;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Reading;

public sealed class CompanionTypesGfx
{
    internal Dictionary<ColorSchemeSwapEnum, uint> SwapDefines { get; } = [];

    internal string AnimClass { get; }
    internal string AnimFile { get; }
    internal uint AsymmetrySwapFlags { get; } = 0;
    internal List<InternalCustomArtImpl> CustomArts { get; } = [];

    public CompanionTypesGfx(XElement element)
    {
        foreach (XElement child in element.Elements())
        {
            string key = child.Name.LocalName;
            string value = child.Value;
            if (key == "GfxType")
            {
                foreach (XElement prop in child.Elements())
                {
                    string propKey = prop.Name.LocalName;
                    string propValue = prop.Value;
                    if (propKey == "AsymmetrySwapFlags")
                    {
                        uint asf = propValue.Split(",").Select(static (flag) =>
                        {
                            if (Enum.TryParse(flag, out BoneTypeEnum result))
                                return 1u << (int)result;
                            return 0u;
                        }).Aggregate((a, v) => a | v);

                        AsymmetrySwapFlags = asf;
                    }
                    else if (propKey.StartsWith("CustomArt"))
                    {
                        CustomArts.Add(ParserUtils.ParseCustomArt(propValue, true, ArtTypeEnum.Companion));
                    }
                    else if (propKey == "AnimFile")
                    {
                        AnimFile = propValue;
                    }
                    else if (propKey == "AnimClass")
                    {
                        AnimClass = propValue;
                    }
                }
            }
            else if (key.EndsWith("_Define"))
            {
                string swap = key[..^"_Define".Length];
                if (!Enum.TryParse(swap, true, out ColorSchemeSwapEnum swapType))
                    throw new ArgumentException($"Invalid swap {swap}");
                uint define = ParserUtils.ParseHexString(value);
                SwapDefines[swapType] = define;
            }
        }

        if (AnimClass is null) throw new ArgumentException("Missing AnimClass");
        if (AnimFile is null) throw new ArgumentException("Missing AnimFile");
    }

    public IGfxType ToGfxType(IColorSchemeType? colorScheme = null)
    {
        InternalGfxImpl gfxResult = new()
        {
            AnimFile = AnimFile,
            AnimClass = AnimClass,
            AsymmetrySwapFlags = AsymmetrySwapFlags,
            CustomArtsInternal = [.. CustomArts],
        };

        if (colorScheme is not null)
        {
            ColorSchemeSwapEnum[] swapTypesList = Enum.GetValues<ColorSchemeSwapEnum>();
            foreach (ColorSchemeSwapEnum swapType in swapTypesList)
            {
                // source is define
                uint sourceColor = SwapDefines.GetValueOrDefault(swapType, 0u);
                if (sourceColor == 0) continue;
                // color exception
                ColorSchemeSwapEnum targetSwapType = swapType;
                // target from scheme
                uint targetColor = colorScheme.GetSwap(targetSwapType);
                if (targetColor == 0) continue;
                InternalColorSwapImpl colorSwap = new()
                {
                    ArtType = ArtTypeEnum.Companion,
                    OldColor = sourceColor,
                    NewColor = targetColor,
                };
                gfxResult.ColorSwapsInternal.Add(colorSwap);
            }
        }

        return gfxResult;
    }
}