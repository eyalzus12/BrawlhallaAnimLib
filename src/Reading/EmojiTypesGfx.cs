using System;
using System.Xml.Linq;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Reading;

public sealed class EmojiTypesGfx
{
    internal string AnimRig { get; }
    internal string? AnimCustomArt { get; }
    internal string? SourceFile { get; }

    public EmojiTypesGfx(XElement element)
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
            else if (key == "SourceFile")
            {
                SourceFile = value;
            }
            // there is also SpriteType which allows using a png, but it's unused rn so im not implementing it
        }

        if (AnimRig is null) throw new ArgumentException("Missing AnimRig");
    }

    public IGfxType ToGfxType()
    {
        InternalGfxImpl gfxResult = new()
        {
            AnimFile = "Animation_Emojis.swf",
            AnimClass = AnimRig,
        };

        if (AnimCustomArt is not null)
        {
            gfxResult.CustomArtsInternal.Add(new InternalCustomArtImpl()
            {
                FileName = SourceFile ?? "Gfx_Emojis.swf",
                Name = AnimCustomArt,
            });
        }

        return gfxResult;
    }
}