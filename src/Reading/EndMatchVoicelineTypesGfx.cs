using System;
using System.Collections.Generic;
using System.Xml.Linq;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Reading;

public sealed class EndOfMatchVoicelineTypesGfx
{
    // why did they hardcode this
    private readonly Dictionary<string, string> CategoryAnimRigs = new()
    {
        ["Default"] = "a__ScreenFanFareB",
        ["Nailbiter"] = "a__ScreenFanFareA",
        ["Steamroll"] = "a__ScreenFanFareA",
        ["Comeback"] = "a__ScreenFanFareA",
        ["Anticlimax"] = "a__ScreenFanFareC",
        ["Draw"] = "a__ScreenFanFareB",
        ["FalseStart"] = "a__ScreenFanFareC",
    };

    internal string Category { get; }
    internal string? AnimCustomArt { get; }

    public EndOfMatchVoicelineTypesGfx(XElement element)
    {
        foreach (XElement child in element.Elements())
        {
            string key = child.Name.LocalName;
            string value = child.Value;

            if (key == "Category")
            {
                Category = value;
            }
            else if (key == "AnimCustomArt")
            {
                AnimCustomArt = value;
            }
        }

        if (Category is null) throw new ArgumentException("Missing Category");
    }

    public IGfxType ToGfxType()
    {
        InternalGfxImpl gfxResult = new()
        {
            AnimFile = "Animation_GameUI.swf",
            AnimClass = CategoryAnimRigs[Category],
        };

        if (AnimCustomArt is not null)
        {
            gfxResult.CustomArtsInternal.Add(new InternalCustomArtImpl()
            {
                FileName = "Gfx_GameUI.swf",
                Name = AnimCustomArt,
            });
        }

        return gfxResult;
    }
}