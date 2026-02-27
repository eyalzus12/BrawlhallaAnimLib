using System;
using System.Xml.Linq;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Reading;

public enum PodiumTeamEnum
{
    None,
    Red,
    Blue,
}

public sealed class PodiumTypesGfx
{
    public string AnimFile { get; }
    public string AnimRig { get; }
    internal string? AnimCustomArt { get; }
    internal string CustomArtTeamRed { get; }
    internal string CustomArtTeamBlue { get; }

    public PodiumTypesGfx(XElement element)
    {
        foreach (XElement child in element.Elements())
        {
            string key = child.Name.LocalName;
            string value = child.Value;

            if (key == "AnimFile")
            {
                AnimFile = value;
            }
            else if (key == "AnimRig")
            {
                AnimRig = value;
            }
            else if (key == "AnimCustomArt")
            {
                AnimCustomArt = value;
            }
            else if (key == "CustomArtTeamRed")
            {
                CustomArtTeamRed = value;
            }
            else if (key == "CustomArtTeamBlue")
            {
                CustomArtTeamBlue = value;
            }
        }

        if (AnimFile is null) throw new ArgumentException("Missing AnimFile");
        if (AnimRig is null) throw new ArgumentException("Missing AnimRig");
        if (CustomArtTeamRed is null) throw new ArgumentException("Missing CustomArtTeamRed");
        if (CustomArtTeamBlue is null) throw new ArgumentException("Missing CustomArtTeamBlue");
    }

    public IGfxType ToGfxType(PodiumTeamEnum team)
    {
        InternalGfxImpl gfxResult = new()
        {
            AnimFile = AnimFile,
            AnimClass = AnimRig,
        };

        if (AnimCustomArt is not null)
        {
            gfxResult.CustomArtsInternal.Add(new InternalCustomArtImpl()
            {
                FileName = AnimFile,
                Name = AnimCustomArt,
            });
        }

        if (team != PodiumTeamEnum.None)
        {
            gfxResult.CustomArtsInternal.Add(new InternalCustomArtImpl()
            {
                FileName = AnimFile,
                Name = team == PodiumTeamEnum.Red ? CustomArtTeamRed : CustomArtTeamBlue,
            });
        }

        return gfxResult;
    }
}