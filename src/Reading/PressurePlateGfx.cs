using System.Collections.ObjectModel;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib;

public sealed class PressurePlateGfx(string pressurePlateName)
{
    public static ReadOnlyCollection<string> KnownPressurePlates { get; } = new([
        "PlateKFPLeft",
        "PlateKFPRight",
        "BloodMoon",
        "PlateDOJOBridge",
        "PlateDOJOWall",
        "PlateDOJOSoftPlat",
    ]);

    public IGfxType ToGfxType()
    {
        InternalGfxImpl gfxResult = new()
        {
            AnimFile = "Animation_GameModes.swf",
            AnimClass = "a__AnimationPressurePlate",
        };

        if (!string.IsNullOrEmpty(pressurePlateName))
        {
            gfxResult.CustomArtsInternal.Add(new InternalCustomArtImpl()
            {
                FileName = "Animation_GameModes.swf",
                Name = pressurePlateName,
            });
        }

        return gfxResult;
    }
}