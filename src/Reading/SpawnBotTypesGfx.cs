using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using BrawlhallaAnimLib.Bones;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Reading;

public sealed class SpawnBotTypesGfx
{
    public string AnimFile { get; }
    public string AnimClass { get; }
    internal bool UseTrueLeftRightTorso { get; } = false;
    internal uint AsymmetrySwapFlags { get; } = 0;
    internal List<InternalCustomArtImpl> CustomArts { get; } = [];
    internal List<InternalColorSwapImpl> ColorSwaps { get; } = [];

    public SpawnBotTypesGfx(XElement element)
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
                        CustomArts.Add(ParserUtils.ParseCustomArt(propValue, true, ArtTypeEnum.Bot));
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
            else if (key == "UseTrueLeftRightTorso")
            {
                UseTrueLeftRightTorso = ParserUtils.ParseBool(value);
            }
        }

        if (AnimClass is null) throw new ArgumentException(element.ToString());
        if (AnimFile is null) throw new ArgumentException("Missing AnimFile");
    }

    public IGfxType ToGfxType()
    {
        InternalGfxImpl gfxResult = new()
        {
            AnimFile = AnimFile,
            AnimClass = AnimClass,
            AsymmetrySwapFlags = AsymmetrySwapFlags,
            UseTrueLeftRightTorso = UseTrueLeftRightTorso,
            CustomArtsInternal = [.. CustomArts],
            ColorSwapsInternal = [.. ColorSwaps],
        };

        return gfxResult;
    }

    public IGfxType ToGfxType(IGfxType gfxType)
    {
        InternalGfxImpl gfxResult = new(gfxType)
        {
            UseTrueLeftRightTorso = UseTrueLeftRightTorso
        };
        gfxResult.CustomArtsInternal.AddRange(CustomArts);
        return gfxResult;
    }
}