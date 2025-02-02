using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SwfLib;
using SwfLib.Data;
using SwfLib.Tags;
using SwfLib.Tags.ControlTags;
using SwfLib.Tags.ShapeTags;

namespace BrawlhallaAnimLib.Loading.Swf;

public class SwfFileData
{
    public SwfFile Swf { get; private init; } = null!;
    public Dictionary<string, ushort> SymbolClass { get; private init; } = [];
    public Dictionary<ushort, DefineSpriteTag> SpriteTags { get; private init; } = [];
    public Dictionary<ushort, ShapeBaseTag> ShapeTags { get; private init; } = [];

    private SwfFileData() { }

    public static SwfFileData CreateFrom(Stream stream)
    {
        SwfFileData swf = new() { Swf = SwfFile.ReadFrom(stream) };

        SymbolClassTag? symbolClass = swf.Swf.Tags.OfType<SymbolClassTag>().FirstOrDefault() ?? throw new ArgumentException("No symbol class in swf");

        foreach (SwfSymbolReference reference in symbolClass.References)
        {
            swf.SymbolClass[reference.SymbolName] = reference.SymbolID;
        }

        foreach (SwfTagBase tag in swf.Swf.Tags)
        {
            if (tag is DefineSpriteTag st)
            {
                swf.SpriteTags[st.SpriteID] = st;
            }
            else if (tag is ShapeBaseTag shape)
            {
                swf.ShapeTags[shape.ShapeID] = shape;
            }
        }

        return swf;
    }
}