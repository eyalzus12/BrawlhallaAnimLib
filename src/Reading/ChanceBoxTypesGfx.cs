using System;
using System.Xml.Linq;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Reading;

public sealed class ChanceBoxTypesGfx
{
    public string BoxAnimFile { get; }
    public string BoxAnimRig { get; }
    internal string? BoxCustomArt { get; }
    public string BoxPodiumAnimFile { get; }
    public string BoxPodiumAnimRig { get; }
    internal string? BoxPodiumCustomArt { get; }

    public ChanceBoxTypesGfx(XElement element)
    {
        foreach (XElement child in element.Elements())
        {
            string key = child.Name.LocalName;
            string value = child.Value;

            if (key == nameof(BoxAnimFile))
            {
                BoxAnimFile = value;
            }
            else if (key == nameof(BoxAnimRig))
            {
                BoxAnimRig = value;
            }
            else if (key == nameof(BoxCustomArt))
            {
                BoxCustomArt = value;
            }
            else if (key == nameof(BoxPodiumAnimFile))
            {
                BoxPodiumAnimFile = value;
            }
            else if (key == nameof(BoxPodiumAnimRig))
            {
                BoxPodiumAnimRig = value;
            }
            else if (key == nameof(BoxPodiumCustomArt))
            {
                BoxPodiumCustomArt = value;
            }
        }

        if (BoxAnimFile is null) throw new ArgumentException("Missing BoxAnimFile");
        if (BoxAnimRig is null) throw new ArgumentException("Missing BoxAnimRig");
        if (BoxPodiumAnimFile is null) throw new ArgumentException("Missing BoxPodiumAnimFile");
        if (BoxPodiumAnimRig is null) throw new ArgumentException("Missing BoxPodiumAnimRig");
    }

    public IGfxType ToBoxGfxType()
    {
        InternalGfxImpl gfxResult = new()
        {
            AnimFile = BoxAnimFile,
            AnimClass = BoxAnimRig,
        };

        if (BoxCustomArt is not null)
        {
            gfxResult.CustomArtsInternal.Add(new InternalCustomArtImpl()
            {
                FileName = BoxAnimFile,
                Name = BoxCustomArt,
            });
        }

        return gfxResult;
    }

    public IGfxType ToBoxPodiumGfxType()
    {
        InternalGfxImpl gfxResult = new()
        {
            AnimFile = BoxPodiumAnimFile,
            AnimClass = BoxPodiumAnimRig,
        };

        if (BoxPodiumCustomArt is not null)
        {
            gfxResult.CustomArtsInternal.Add(new InternalCustomArtImpl()
            {
                FileName = BoxPodiumAnimFile,
                Name = BoxPodiumCustomArt,
            });
        }

        return gfxResult;
    }
}