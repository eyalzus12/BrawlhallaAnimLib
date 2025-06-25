using System;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Reading;

public sealed class AvatarTypesGfx
{
    internal string FlagArtFile { get; }
    internal string FlagArt { get; }
    internal string? OverrideIconFile { get; }
    internal string? BitmapFileName { get; }
    internal string? SpriteFilePath { get; }

    public AvatarTypesGfx(ICsvRow row)
    {
        foreach ((string key, string value) in row.ColEntries)
        {
            if (value == "") continue;

            if (key == "FlagArtFile")
            {
                FlagArtFile = value;
            }
            else if (key == "FlagArt")
            {
                FlagArt = value;
            }
            else if (key == "OverrideIconFile")
            {
                OverrideIconFile = value;
            }
            else if (key == "BitmapFileName")
            {
                BitmapFileName = value;
            }
            else if (key == "SpriteFilePath")
            {
                SpriteFilePath = value;
            }
        }

        if (FlagArtFile is null) throw new ArgumentException("Missing FlagArtFile");
        if (FlagArt is null) throw new ArgumentException("Missing FlagArt");
    }

    public ICustomArt ToFlagCustomArt()
    {
        return new InternalCustomArtImpl()
        {
            FileName = FlagArtFile,
            Name = FlagArt,
            Type = ArtTypeEnum.Flag,
        };
    }
}