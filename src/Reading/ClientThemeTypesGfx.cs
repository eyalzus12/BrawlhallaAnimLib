using System;
using System.Xml.Linq;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Reading;

public sealed class ClientThemeTypesGfx
{
    internal string? AnimCustomArt { get; }
    internal string AnimRig { get; }

    public ClientThemeTypesGfx(XElement element)
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
            AnimFile = "Animation_ClientThemeLogos.swf",
            AnimClass = AnimRig,
        };

        if (AnimCustomArt is not null)
        {
            gfxResult.CustomArtsInternal.Add(new InternalCustomArtImpl()
            {
                FileName = "Gfx_ClientThemeLogos.swf",
                Name = AnimCustomArt,
            });
        }

        return gfxResult;
    }
}