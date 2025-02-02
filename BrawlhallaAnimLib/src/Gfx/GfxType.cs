using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BrawlhallaAnimLib.Gfx;

public interface IGfxType
{
    public enum AsymmetrySwapFlagEnum
    {
        HAND = 1,
        FOREARM = 2,
        ARM = 3,
        SHOULDER = 4,
        LEG = 5,
        SHIN = 6,
        FOOT = 7,
        // 8 isn't settable through xml. it is TORSO.
        GAUNTLETHAND = 9,
        GAUNTLETFOREARM = 10,
        PISTOL = 11,
        KATAR = 12,
        // 13 isn't settable through xml. it is JAW.
        // 14 isn't settable through xml. it is EYES.
        // 15 isn't settable through xml. might be BOOTS.
        // 16 isn't settable through xml. it is MOUTH.
        // 17 isn't settable through xml. it is HAIR.
    }

    string AnimFile { get; set; }
    string AnimClass { get; set; }
    double AnimScale { get; set; }
    uint Tint { get; set; }

    bool HasAsymmetrySwapFlag(AsymmetrySwapFlagEnum flag);

    IEnumerable<ICustomArt> CustomArts();
    IEnumerable<IColorSwap> ColorSwaps();

    // these are set in costumeTypes.csv.
    bool UseRightTorso { get; set; }
    bool UseRightJaw { get; set; }
    bool UseRightEyes { get; set; }
    bool UseRightMouth { get; set; }
    bool UseRightHair { get; set; }
    bool UseRightForearm { get; set; }
    bool UseRightShoulder1 { get; set; }
    bool UseRightLeg1 { get; set; }
    bool UseRightShin { get; set; }
    bool UseTrueLeftRightHands { get; set; }
    bool TryGetBoneOverride(string bone, [MaybeNullWhen(false)] out string newBone);
    // these are set in weaponSkinType.csv
    bool UseRightGauntlet { get; set; }
    bool UseRightKatar { get; set; }
}