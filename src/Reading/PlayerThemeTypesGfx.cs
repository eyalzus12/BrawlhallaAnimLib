using System;
using System.Xml.Linq;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Reading;

public sealed class PlayerThemeTypesGfx
{
    internal string AnimRig { get; }
    internal string? AnimCustomArt { get; }

    public PlayerThemeTypesGfx(XElement element)
    {
        foreach (XElement child in element.Elements())
        {
            string key = child.Name.LocalName;
            string value = child.Value;

            if (key == "AnimRig")
            {
                AnimRig = value;
            }
            else if (key == "AnimCustomArt")
            {
                AnimCustomArt = value;
            }
        }

        if (AnimRig is null) throw new ArgumentException("Missing AnimRig");
    }

    public IGfxType ToGfxType()
    {
        InternalGfxImpl gfxResult = new()
        {
            AnimFile = "Animation_PlayerThemes.swf",
            AnimClass = AnimRig,
        };

        if (AnimCustomArt is not null)
        {
            gfxResult.CustomArtsInternal.Add(new InternalCustomArtImpl()
            {
                FileName = "Gfx_PlayerThemes.swf",
                Name = AnimCustomArt,
            });
        }

        return gfxResult;
    }
}